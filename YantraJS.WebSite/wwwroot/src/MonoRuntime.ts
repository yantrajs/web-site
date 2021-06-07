let monoRuntime: MonoRuntime;

interface IModule {
    _malloc?: any;
    cwrap?: any;
    FS_createPath?: any;
    FS_createDataFile?: any;
    getValue?: any;
    UTF8ToString?: any;
    _free?: any;
    setValue?: any;
    onRuntimeInitialized: any;
    bclLoadingDone: any;
}

class WebAssemblyApp {

    constructor() {
        this.initAsync = new Promise(() => {
            
        });
    }

    init() {
        
    }


}

const theApp = new WebAssemblyApp();

class MonoRuntime {
    public loadRuntime = Module.cwrap ("mono_wasm_load_runtime", null, ["string", "number"]);
    public assemblyLoad = Module.cwrap ("mono_wasm_assembly_load", "number", ["string"]);
    public findClass = Module.cwrap ("mono_wasm_assembly_find_class", "number", ["number", "string", "string"]);
    public findMethod = Module.cwrap ("mono_wasm_assembly_find_method", "number", ["number", "string", "number"]);
    public invokeMethod = Module.cwrap ("mono_wasm_invoke_method", "number", ["number", "number", "number"]);
    public monoStringGetUTF8 = Module.cwrap ("mono_wasm_string_get_utf8", "number", ["number"]);
    public monoString = Module.cwrap ("mono_wasm_string_from_js", "number", ["string"]);

    constructor() {
        this.loadRuntime ("managed", 1);
        theApp.init ();
    }

    public convertToString(monoObj) {
        if (monoObj === 0) {
            return null;
        }
        const raw = this.monoStringGetUTF8 (monoObj);
        const res = Module.UTF8ToString (raw);
        Module._free (raw);
        return res;
    }

    public callMethod(method, thisArg, args) {
        const argsMem = Module._malloc (args.length * 4);
        const exThrow = Module._malloc (4);
        for (let i = 0; i < args.length; ++i) {
            Module.setValue (argsMem + i * 4, args [i], "i32");
        }
        Module.setValue (exThrow, 0, "i32");

        const res = this.invokeMethod (method, thisArg, argsMem, exThrow);

        const exRes = Module.getValue (exThrow, "i32");

        Module._free (argsMem);
        Module._free (exThrow);

        if (exRes !== 0) {
            const msg = this.convertToString (res);
            throw new Error (msg);
        }

        return res;
    }
}

let Module: IModule = {
    onRuntimeInitialized() {

        Module.FS_createPath ("/", "managed", true, true);

        let pending = 0;
        Promise.all(this.assemblies.map (async (asmName) => {
            ++pending;
            const blob = await fetch ("managed/" + asmName, { credentials: "same-origin" }).then ((response) => {
                if (!response.ok) {
                    throw new Error("failed to load Assembly '" + asmName + "'");
                }
                return response.arrayBuffer();
            });
            const asm = new Uint8Array (blob);
            Module.FS_createDataFile ("managed/" + asmName, null, asm, true, true, true);
        })).then(() => this.bclLoadingDone());
    },

    bclLoadingDone() {
        monoRuntime = new MonoRuntime();
    }
};
