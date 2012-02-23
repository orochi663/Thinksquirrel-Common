#!/usr/bin/perl

use strict;
use Getopt::Long;
use Term::ANSIColor;
use File::Basename;
use File::Spec;
chdir (File::Spec->rel2abs (dirname($0)));

our $compiler = "/Applications/Unity/Unity.app/Contents/Frameworks/Mono/bin/gmcs";
our $assemblyUnityEngine = "/Applications/Unity/Unity.app/Contents/Frameworks/Managed/UnityEngine.dll";
our $assemblyUnityEditor = "/Applications/Unity/Unity.app/Contents/Frameworks/Managed/UnityEditor.dll";

my $optionRelease = 1;

GetOptions (
	"release" => \$optionRelease
);

my $debugOptions = $optionRelease == 0 ? "-d:DEBUG" : "";

sub BuildAssembly
{
	our $compiler;

	my $target = shift;
	my $out = shift;
	my $arguments = shift;
	my $source = shift;

	print ("Compiling $out\n");
	print color ("blue"), "$compiler -target:$target -out:$out $arguments $source\n";
	print color ("red");
	system ("$compiler -target:$target -out:$out $arguments $source") and die ("Compilation of $out failed.");
	print color ("reset");
}
BuildAssembly ("library", "bin/ThinksquirrelCommon.Runtime.dll", "'Assets/Plugins/Thinksquirrel Common/Source/*.cs'", "-v -recurse:'Assets/Plugins/Thinksquirrel Common/Source/*.cs' -d:RUNTIME $debugOptions -r:$assemblyUnityEngine");
BuildAssembly ("library", "bin/ThinksquirrelCommon.Editor.dll", "'Assets/Editor/Thinksquirrel Common/*.cs'", "-v -recurse:'Assets/Editor/Thinksquirrel Common/Source/*.cs' -d:RUNTIME $debugOptions -r:bin/ThinksquirrelCommon.Runtime.dll,$assemblyUnityEngine,$assemblyUnityEditor");