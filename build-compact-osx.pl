#!/usr/bin/perl

# Thinksquirrel Common Library build script for Unity projects (OSX)
# Place this at the project root.

use strict;
use Getopt::Long;
use Term::ANSIColor;
use File::Basename;
use File::Spec;
chdir (File::Spec->rel2abs (dirname($0)));

our $compiler = "/Applications/Unity/Unity.app/Contents/Frameworks/Mono/bin/gmcs";
our $assemblyUnityEngine = "/Applications/Unity/Unity.app/Contents/Frameworks/Managed/UnityEngine.dll";
our $assemblyUnityEditor = "/Applications/Unity/Unity.app/Contents/Frameworks/Managed/UnityEditor.dll";
our $sourceThinksquirrelCommon = "'Assets/Plugins/Thinksquirrel Common/Source/*.cs'";
our $sourceThinksquirrelEditor = "'Assets/Editor/Thinksquirrel Common/Source/*.cs'";

my $optionRelease = 1;

GetOptions (
	"release" => \$optionRelease
);

my $debugOptions = $optionRelease == 0 ? "-d:DEBUG,COMPACT -optimize" : "-d:COMPACT -optimize";

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

BuildAssembly ("library", "'bin/compact/Plugins/Thinksquirrel Common/ThinksquirrelCommon.Runtime.dll'", $sourceThinksquirrelCommon, "-v -recurse:$sourceThinksquirrelCommon -d:RUNTIME $debugOptions -r:$assemblyUnityEngine");
BuildAssembly ("library", "'bin/compact/Editor/Thinksquirrel Common/ThinksquirrelCommon.Editor.dll'", $sourceThinksquirrelEditor, "-v -recurse:$sourceThinksquirrelEditor -d:RUNTIME $debugOptions -r:'bin/compact/Plugins/Thinksquirrel Common/ThinksquirrelCommon.Runtime.dll',$assemblyUnityEngine,$assemblyUnityEditor");