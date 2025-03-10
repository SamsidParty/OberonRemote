import { useState } from "react";
import "./App.css";
import "./GamepadInput.jsx";
import GamepadPreview from "./components/GamepadPreview.jsx";

window.serverStatus = {
    ListenIP: "..."
}

async function UpdateStatus() {
    window.serverStatus = await igniteView.commandBridge.getServerStatus();
    !!window.rerender ? window.rerender() : console.log(0);
}

setInterval(UpdateStatus, 2000);
UpdateStatus();

function App() {

    var [renderID, setRenderID] = useState(0);
    window.rerender = () => setRenderID(renderID + 1);

    return (
        <>
            <h1 className="listenIP">IP Address: {window.serverStatus.ListenIP}</h1>
            <GamepadPreview></GamepadPreview>
        </>
    );
}

export default App;
