/*
 * Gemaakt met SharpDevelop.
 * Gebruiker: ServicePC
 * Datum: 10-8-2010
 * Tijd: 17:26
 * 
 * Dit sjabloon wijzigen: Extra | Opties |Coderen | Standaard kop bewerken.
 */
using System;
using System.IO;
using NUnit.Framework;

namespace Aubergine.Tests
{
	[TestFixture]
	public class NUnitBDDFixture : Aubergine.NUnitBDD.Fixture
	{
		public NUnitBDDFixture()
		{
		}
		
		public override System.Collections.Generic.IEnumerable<string> SpecFiles {
			get {
				yield return Directory.GetCurrentDirectory()+"/Website/Stories/Make sure my website gets enough traffic.txt";
				yield return Directory.GetCurrentDirectory()+"/Accounts/Stories/Transfer money between accounts.txt";
			}
		}
	}
}
