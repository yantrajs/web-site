# CSX Modules

Yantra supports loading CSX files as modules which can be used in JavaScript. There are two ways you can export your code in CSX Module.

Since Yantra is built on .NET, we have given csx module higher priority. So if there are two files with same name and `.csx` and `.js`, `.csx` module has higher priority. This is done so you can create unified module and in absence of Yantra, normal `.js` module can be loaded.

1. Module Function
2. Export Attributes

## Module Function

CSX Module should return a Module Loader delegate as shown below.

```c#
#r "nuget: YantraJS.Core"
using System;
using WebAtoms.CoreJS.Core;

static void Module(
    JSValue exports, 
    JSValue require, 
    JSValue module, 
    string __filename, 
    string __dirname) {

    // ... code to load module

}

// return delegate to local function Module
return (JSModuleDelegate)Module;
```
> Please note, `return` statement is outside `Module` function.

Lets assume you are creating a Vector module...

```c#
// file "vector.csx"
#r "nuget: YantraJS.Core"
using System;
using WebAtoms.CoreJS.Core;

class Vector {
    public double X {get;set;}
    public double Y {get;set;}
    public double Z {get;set;}

    public Vector() {

    }
    public Vector(double x, double y, double z) {
        this.X = x;
        this.Y = y;
        this.Z = z;
    }

    public Vector Add(Vector v) {
        // ...
    }
}

static void Module(
    JSValue exports, 
    JSValue require, 
    JSValue module, 
    string __filename, 
    string __dirname) {

    // ... code to load module
    exports["default"] = typeof(Vector).Marshal();
}

// return delegate to local function Module
return (JSModuleDelegate)Module;
```

JavaScript file to consume CSX module.
```javascript
var Vector = require("./vector").default;

var v1 = new Vector(1,2,3);
var v2 = v1.add(new Vector(2,3,4));
```

Above code will export "default" named export. To make wildcard export, you can set property `exports` on `module` as shown below.

```c#

static void Module(
    JSValue exports, 
    JSValue require, 
    JSValue module, 
    string __filename, 
    string __dirname) {

    module["exports"] = typeof(Vector).Marshal();
}

// return delegate to local function Module
return (JSModuleDelegate)Module;
```

### Consume external JavaScript module

In CSX Module, you can consume any other javascript module.

```c#

static void Module(
    JSValue exports, 
    JSValue require, 
    JSValue module, 
    string __filename, 
    string __dirname) {

    // following is equivalent of
    // require("nameOfModule")
    var @this = JSUndefined.Value;
    var args = new Arguments(@this, "nameOfModule");
    var externalModule = require.InvokeFunction(args);

    // ... code to load module
    exports["default"] = typeof(Vector).Marshal();
}
```

### Consume external CSX Module

CSX does allow loading external CSX content, however, Yantra module loader does not support loading external CSX directly, you have to load them exactly as how you would load JavaScript module. There is no difference between CSX module or JavaScript module.

## Export Attributes

In absence of Return statement, Module loader will look up for defined types decorated with `Export` attribute.

Above CSX module can be rewritten simply as follow,

```c#
// file "vector.csx"
#r "nuget: YantraJS.Core"
using System;
using WebAtoms.CoreJS.Core;

[DefaultExport]
// OR
[Export("default")]
class Vector {
    public double X {get;set;}
    public double Y {get;set;}
    public double Z {get;set;}

    public Vector() {

    }
    public Vector(double x, double y, double z) {
        this.X = x;
        this.Y = y;
        this.Z = z;
    }

    public Vector Add(Vector v) {
        // ...
    }
}
```

There are two attributes. `Export`, `DefaultExport`. `Export` attribute takes a string parameter that acts as export literal.

`Export` attribute without any name is considered as wildcard export.