const { View } = require("AspNetCore");
const clr = require("clr").default;

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
    <h1>YantraJS (Machine in Sanskrit) is a JavaScript Engine</h1>
    <p>
        Please visit GitHub repo at <a href="https://github.com/yantrajs/yantra">YantraJS</a>
        <ul>
            <li><a href="https://github.com/yantrajs/yantra/discussions">Discussions</a></li>
            <li><a href="https://github.com/yantrajs/yantra/issues">Issues</a></li>
            <li><a href="https://github.com/yantrajs/yantra/wiki">Documentation</a></li>
        </ul>

        To Purchase license, click on the following links.
        <table>
            <tr>
                <td>YantraJS Standard License for $99 per Application/Website</td>
                <td><a href="https://www.webatoms.in/purchase/yantrajs.sdk.standard"><button>Buy Now</button></a></td>
            </tr>
            <tr>
                <td>YantraJS Enterprise License for $999 per Application/Website</td>
                <td><a href="https://www.webatoms.in/purchase/yantrajs.sdk.enterprise"><button>Buy Now</button></a></td>
            </tr>
        </table>
    </p>
    <p>
        ${content}
    </p>
</body>
</html>
`;
    }

}