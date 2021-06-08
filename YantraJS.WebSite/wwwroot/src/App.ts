/// <reference path="./MonoRuntime.ts"/>
/// <reference path="../wasm/mono.js"/>

interface IMonoMessage {
    id: number;
    parse?: string;
    evaluate?: string;
}

class YantraJSRunner {
    public static async evaluate(text?: string): Promise<string> {
        await theApp.initAsync;
        return text;
    }
}

onmessage = (e) => {
    const { id, evaluate } = e.data as IMonoMessage;
    const origin = e.origin;
    if (evaluate) {
        YantraJSRunner.evaluate(evaluate)
        .then((result) => {
            postMessage({
                id,
                result
            }, origin);
        })
        .catch((error) => {
            postMessage({
                id,
                error
            }, origin);
        });
    }
};
