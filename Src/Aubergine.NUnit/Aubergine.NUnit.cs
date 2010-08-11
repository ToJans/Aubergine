/*
 * Gemaakt met SharpDevelop.
 * Gebruiker: ServicePC
 * Datum: 10-8-2010
 * Tijd: 15:36
 * 
 * Dit sjabloon wijzigen: Extra | Opties |Coderen | Standaard kop bewerken.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Aubergine.Interfaces;
using Aubergine.Services;
using Aubergine.Services.Extractors;
using NUnit.Framework;

namespace Aubergine.NUnitBDD
{
	[TestFixture]
	public abstract class Fixture
	{
		public abstract IEnumerable<string> SpecFiles {get;}
		
		public IEnumerable Specs
		{
			get {
				var storyextractor = new TextStoryExtractor();
				var dslextractor = new TextDSLExtractor();
				
				foreach (var fn in SpecFiles)
				{
					foreach (var line in File.ReadAllLines(fn))
					{
						if (string.IsNullOrEmpty(line) || line.Trim() == "")
							continue;
						if (dslextractor.ParseLine(line))
							continue;
						else if (storyextractor.ParseLine(line))
							continue;
					}
				}
				var runner = new SpecRunner();
				foreach (var sc in storyextractor.Stories)
				{
					var dsl = runner.GetDSL(sc.Story, dslextractor.DSLs);
					dsl = dsl ?? dslextractor.DSLs["default"];
					foreach( var v in runner.RunStory(sc.Story,dsl,sc.ColumnToken).Children)
					{
						var td = new TestCaseData(v).SetName(v.Description).SetDescription(v.StatusInfo);
						if (v.Status==null)
							td.Ignore();
						yield return td;
					}
				}
			}
		}

		[Test,TestCaseSource("Specs")]
		public void TestMethod(ISpecElement e)
		{
			Assert.That(e.Status == true,e.Description+" Failed");
		}
	}


}
