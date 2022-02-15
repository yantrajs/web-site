/**
 * Load `clr` and `AspNetCore` module.
 * 
 * 1. Derive a class from `View` and return it as default exprot
 * 2. Load class `YantraJS.WebSite.Services.MarkdownService` from `YantraJS.WebSite` assembly
 * 3. Resolve service with MarkdownService class retrieved in step 2
 * 4. Call processMarkdownAsync method and await on it
 * 5. Return text to be rendered
 */

 const clr = require("clr").default;
 const { View } = require("AspNetCore");

const markdownServiceType = clr.getClass("YantraJS.WebSite.Services.MarkdownService, YantraJS.WebSite");

export default class IndexView extends View {

    async render() {

        const ms = this.services.resolve(markdownServiceType);

        const content = await ms.processMarkdownAsync("https://raw.githubusercontent.com/yantrajs/yantra/main/README.md");

        return `
<!DOCTYPE html>
<html lang="en">
<head>
    <title>YantraJS JavaScript Engine written in .NET Standard</title>
    <meta charset="utf-8">
    <style>
    * {
        font-family: -apple-system, BlinkMacSystemFont, "Segoe UI Variable", "Segoe UI", system-ui, ui-sans-serif, Helvetica, Arial, sans-serif, "Apple Color Emoji", "Segoe UI Emoji"
    }
    body {
        overflow-x: hidden;
    }
    button {
        border-radius: 5px;
        background-color: limegreen;
        color: white;
        border-width: 0;
        padding: 5px;
        margin: 6px;
        padding-left: 10px;
        padding-right: 10px;
    }
      .background--custom {
        background-color: #FFFFFF;
        width: 100vw;
        height: 100vh;
        position: absolute;
        overflow: hidden;
        z-index: -2;
        top: 0;
        left: 0;
      }
      canvas#canvas {
        z-index: -1;
        position: absolute;
        width: 100%;
        height: 60%;
        transform: rotate(-12deg) scale(2) translateY(-56%);
        --gradient-color-1: #EF008F;
        --gradient-color-2: #F46E6E;
        --gradient-color-3: #7038ff;
        --gradient-color-4: #A427FF;
        --gradient-speed: 0.000006;
      }
    </style>
</head>
<body>
    <div class="background--custom">
      <canvas id="canvas" />
    </div>
    <div style="width: 700px; margin-left: auto; margin-top:20px; margin-bottom: 20px; margin-right: auto; background-color: #cbcbe35e; padding: 20px; border-radius: 20px;">
        <h1>YantraJS (Machine in Sanskrit)</h1>
        <p>
            Please visit GitHub repo at <a href="https://github.com/yantrajs/yantra">YantraJS</a>
            <ul>
                <li><a href="https://github.com/yantrajs/yantra/discussions">Discussions</a></li>
                <li><a href="https://github.com/yantrajs/yantra/issues">Issues</a></li>
                <li><a href="https://github.com/yantrajs/yantra/wiki">Documentation</a></li>
            </ul>
        </p>
        <p>
            ${content}
        </p>
        <div style="width: 700px">
            <h3>Support YantraJS</h3>
            <a href="https://patreon.com/yantrajs">patreon.com/yantrajs</a>
        </div>
    </div>
    <script src="https://cdn.jsdelivr.net/gh/greentfrapp/pocoloco@minigl/minigl.js"></script>
    <script>
    var gradient = new Gradient();
    gradient.initGradient("#canvas");
    </script>
</body>
</html>
`;
    }

}