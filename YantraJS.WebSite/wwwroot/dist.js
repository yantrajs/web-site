var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
let monoRuntime;
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
    constructor() {
        this.loadRuntime = Module.cwrap("mono_wasm_load_runtime", null, ["string", "number"]);
        this.assemblyLoad = Module.cwrap("mono_wasm_assembly_load", "number", ["string"]);
        this.findClass = Module.cwrap("mono_wasm_assembly_find_class", "number", ["number", "string", "string"]);
        this.findMethod = Module.cwrap("mono_wasm_assembly_find_method", "number", ["number", "string", "number"]);
        this.invokeMethod = Module.cwrap("mono_wasm_invoke_method", "number", ["number", "number", "number"]);
        this.monoStringGetUTF8 = Module.cwrap("mono_wasm_string_get_utf8", "number", ["number"]);
        this.monoString = Module.cwrap("mono_wasm_string_from_js", "number", ["string"]);
        this.loadRuntime("managed", 1);
        theApp.init();
    }
    convertToString(monoObj) {
        if (monoObj === 0) {
            return null;
        }
        const raw = this.monoStringGetUTF8(monoObj);
        const res = Module.UTF8ToString(raw);
        Module._free(raw);
        return res;
    }
    callMethod(method, thisArg, args) {
        const argsMem = Module._malloc(args.length * 4);
        const exThrow = Module._malloc(4);
        for (let i = 0; i < args.length; ++i) {
            Module.setValue(argsMem + i * 4, args[i], "i32");
        }
        Module.setValue(exThrow, 0, "i32");
        const res = this.invokeMethod(method, thisArg, argsMem, exThrow);
        const exRes = Module.getValue(exThrow, "i32");
        Module._free(argsMem);
        Module._free(exThrow);
        if (exRes !== 0) {
            const msg = this.convertToString(res);
            throw new Error(msg);
        }
        return res;
    }
}
let Module = {
    onRuntimeInitialized() {
        Module.FS_createPath("/", "managed", true, true);
        let pending = 0;
        Promise.all(this.assemblies.map((asmName) => __awaiter(this, void 0, void 0, function* () {
            ++pending;
            const blob = yield fetch("managed/" + asmName, { credentials: "same-origin" }).then((response) => {
                if (!response.ok) {
                    throw new Error("failed to load Assembly '" + asmName + "'");
                }
                return response.arrayBuffer();
            });
            const asm = new Uint8Array(blob);
            Module.FS_createDataFile("managed/" + asmName, null, asm, true, true, true);
        }))).then(() => this.bclLoadingDone());
    },
    bclLoadingDone() {
        monoRuntime = new MonoRuntime();
    }
};
//# sourceMappingURL=dist.js.map