#!/usr/bin/perl

# Thinksquirrel Common Library build script (OSX)
# build-common-osx.pl
# Thinksquirrel Software Common Libraries
#  
# Authors:
#       Josh Montoute <josh@thinksquirrel.com>
#		Emil "AngryAnt" Johansen <Twitter: @AngryAnt>
#
# Original code adapted from the blog post here:
# http://angryant.com/2011/02/02/assembling-and-assimilating/
#
# Copyright (c) 2011-2012, Thinksquirrel Software, LLC
# All rights reserved.
#
# Redistribution and use in source and binary forms, with or without modification, 
# are permitted provided that the following conditions are met:
#
# Redistributions of source code must retain the above copyright notice,
# this list of conditions and the following disclaimer.
# 
# THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND 
# ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES 
# OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT 
# SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
# SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT 
# OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
# HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, 
# OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, 
# EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

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

my $debugOptions = $optionRelease == 0 ? "-d:DEBUG -optimize" : "-optimize";

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

BuildAssembly ("library", "'bin/common/Plugins/Thinksquirrel Common/ThinksquirrelCommon.Runtime.dll'", $sourceThinksquirrelCommon, "-v -recurse:$sourceThinksquirrelCommon $debugOptions -r:$assemblyUnityEngine");
BuildAssembly ("library", "'bin/common/Editor/Thinksquirrel Common/ThinksquirrelCommon.Editor.dll'", $sourceThinksquirrelEditor, "-v -recurse:$sourceThinksquirrelEditor $debugOptions -r:'bin/common/Plugins/Thinksquirrel Common/ThinksquirrelCommon.Runtime.dll',$assemblyUnityEngine,$assemblyUnityEditor");