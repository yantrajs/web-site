const { View } = require("AspNetCore");

export default class IndexView extends View {

    render() {
        return `Demo ${this.model.message}`;
    }

}