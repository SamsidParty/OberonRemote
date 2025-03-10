import { useState } from "react";
import "./App.css";
import "./GamepadInput.jsx";
import GamepadPreview from "./components/GamepadPreview.jsx";


function App() {

    var [renderID, setRenderID] = useState(0);
    window.rerender = () => setRenderID(renderID + 1);

    return (
        <>
            <GamepadPreview></GamepadPreview>
        </>
    );
}

export default App;
