src_path = "../src"
project_name = "Aubergine"
solution_file = "${src_path}/${project_name}.sln"
configuration = "release"
test_assemblies = "${src_path}/${project_name}.Tests/bin/${configuration}/${project_name}.Tests.dll"

target default, (init, compile, test, deploy, package):
  pass

target ci, (init, compile, coverage, deploy, package):
  pass

target init:
  exec("cleanup.cmd")

  rmdir("../redist")

desc "Compiles the solution"
target compile:
  msbuild(file: solution_file, configuration: configuration)

desc "Executes tests"
target test:
  nunit(assembly: test_assemblies, toolPath: "${src_path}/lib/NUnit/nunit-console.exe")

desc "Copies the binaries to the 'build' directory"
target deploy:
  print "Copying to build dir"

  with FileList("${src_path}/Aubergine.ConsoleRunner/bin/${configuration}"):
    .Include("*.{dll,exe}")
    .ForEach def(file):
      file.CopyToDirectory("../Build/${configuration}")

  with FileList("${src_path}/Aubergine.NUnit/bin/${configuration}"):
    .Include("*.{dll,exe}")
    .ForEach def(file):
      file.CopyToDirectory("../Build/${configuration}")
      
desc "Creates zip package"
target package:
  zip("../Build/${configuration}", "../Build/${project_name}_${configuration}.zip")

desc "Runs code coverage with ncover (only runs on build server)"
target coverage:
  ncover_path = "C:/Program Files (x86)/ncover"
  app_assemblies = ("Phantom.Core",)
  teamcity_launcher = env("teamcity.dotnet.nunitlauncher")
  
  with ncover():
    .toolPath = "${ncover_path}/NCover.console.exe"
    .reportDirectory = "build/Coverage"
    .workingDirectory = "src/Phantom.Tests/bin/${configuration}"
    .applicationAssemblies = app_assemblies
    .program = "${teamcity_launcher} v2.0 x86 NUnit-2.4.6"
    .testAssembly = "Phantom.Tests.dll"
    .excludeAttributes = "Phantom.Core.ExcludeFromCoverageAttribute;System.Runtime.CompilerServices.CompilerGeneratedAttribute"
  
  with ncover_explorer():
    .toolPath = "${ncover_path}/NCoverExplorer.console.exe"
    .project = "Phantom"
    .reportDirectory = "build/Coverage"