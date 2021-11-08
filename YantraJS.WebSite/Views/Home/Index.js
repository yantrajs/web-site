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
<html>
<head>
    <title>YantraJS JavaScript Engine written in .NET Standard</title>
    <style>
    * {
        font-family: -apple-system, BlinkMacSystemFont, "Segoe UI Variable", "Segoe UI", system-ui, ui-sans-serif, Helvetica, Arial, sans-serif, "Apple Color Emoji", "Segoe UI Emoji"
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
    </style>
</head>
<body>
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
    <p>
        <h3>Sponsor - Web Atoms (React Native alike) for Xamarin Forms</h3>
        <p>This project is supported by Web Atoms, Web Atoms provides JSX+JavaScript bridge for Xamarin Forms 
        which lets you write Xamarin Forms apps in React Native style.</p>
        <a href="https://www.webatoms.in" target="_blank"><img src="https://cdn.jsdelivr.net/gh/web-atoms/resources@v1.0.1/ads/landscape/JSX-for-Xamarin.Forms.gif" style="border:none"/></a>
    </p>
</body>
</html>
`;
    }

}