import Orb from "./Orb.jsx";

function GetControllerType() {

    var primaryGamepad = null;

    for (var gamepad of navigator.getGamepads()) {
        if (!gamepad || !gamepad.connected) continue;
        primaryGamepad = gamepad;
    }

    if (primaryGamepad == null) {
        return "Missing";
    }
    else if (primaryGamepad.id.toLowerCase().includes("dualsense")) {
        return "PS5";
    }

    return "Generic";
}

function GetControllerImage() {
    return `/Images/Controller_${GetControllerType()}.png`
}

export default function GamepadPreview() {
    return (
        <div className="gamepadPreview">
            <Orb
                hoverIntensity={0}
                rotateOnHover={true}
                hue={0}
                forceHoverState={false}
            />
            <img src={GetControllerImage()}></img>
        </div>
    )
}