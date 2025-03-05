import { SendControlPacket } from "./SocketClient";

var loopStarted = false;

window.addEventListener("gamepadconnected", (e) => {

    if (!loopStarted) {
        loopStarted = true;
        requestAnimationFrame(GamepadLoop);
    }

    console.log("Gamepad" + e.gamepad.index + " has connected");
});


function GamepadLoop() {
    loopStarted = true;

    for (var gamepad of navigator.getGamepads()) {
        if (!gamepad || !gamepad.connected) continue;
        SendControlPacket(gamepad);
    }
  
    requestAnimationFrame(GamepadLoop);
}

