# C# Option Parser

| Windows | Linux / OS X | Coverage |
|:-:|:-:|:-:|
| [![Build Status](https://ci.appveyor.com/api/projects/status/y4aadk071ynavn1r?svg=true)](https://ci.appveyor.com/project/Strnadj/csharpoptparser) | [![Build Status](https://travis-ci.org/Strnadj/CsharpOptParser.svg?branch=master)](https://travis-ci.org/Strnadj/CsharpOptParser) | [![Coverage Status](https://coveralls.io/repos/Strnadj/CsharpOptParser/badge.svg?branch=master&service=github)](https://coveralls.io/github/Strnadj/CsharpOptParser?branch=master) |


## Info

C# Simple Option parser is inspired from ruby OptParser class
([rdoc/OptionParser](http://ruby-doc.org/stdlib-2.0.0/libdoc/optparse/rdoc/OptionParser.html)). This work has been created as part of Researching Work on University of West Bohemia ([zcu.cz](http://zcu.cz)).

## Installation

Add C# files into your project, or add project itselfs and add refereces.

## Usage

You can find usage in tests or in example bellow. CsharpOpt class provides
basic *fluent interfaces* for creating option definitions. This usage is the same as it in java option parser.

### Difference between Option and Path/Expression

* *Option* - classic option (ex: -v, --verbose)
* *Path/Expression* - string or path after option fields

```cli
cd *path/expression*
cd ~/strnadj/Projects
cp *option* *path/expression* *path/expression*
cp -R ~/strnadj/Projects ~/strnadj/Projects_bacup/
```

## Example

We want basic *ls* command with options:

* Only files
* Only directories
* With hidden files
* Help
* And optional path

### Create parser definition

```csharp
OptParser options = OptParser.createOptionParser("ls", "Show directory
contents")
    .addOption('f', "files", OptParser.OPTIONAL, "", "Just files")
    .addOption('d', "directories", OptParser.OPTIONAL, "", "Just
directories")
    .addOption('h', "help", OptParser.OPTIONAL, "", "Show this help")
    .addPathOrExpression("path", OptParser.OPTIONAL, ".", "Directory to
listing");
```

### Create option with required value?

```csharp
.addOptionRequiredValue('t', "target", OptParser.OPTIONAL, null, "Target
folder")
```


### Get help?

```csharp
Console.WriteLine(options.getHelp());
```


```bash
Usage: ls [options] "path"

Optional options:
	-f, --files         Just files
	-d, --directories   Just directories	
	-h, --help          Show this help
```

### Parse parameters

```csharp
try {
    options.parseArguments(args);
} catch(Exception e) {
    Console.WriteLine(e.Message);
    Environment.Exit(-1);
}
```

### Getting parameters

```csharp
// Get if parameter was set
if (options.getOption("help") != null) {
    // Parameter help was set
}

// Values?
if (options.getOption("directories") != null) {
    options.getOption("directories").value();
}

```

## How to contribute?

1. Fork it!
2. Do your changes!
3. Create pull-request and open issue!


Thanks Strnadj :)

