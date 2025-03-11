
var loopStarted = false;

window.addEventListener("gamepadconnected", (e) => {
    !!window.rerender ? window.rerender() : console.log(0);

    if (!loopStarted) {
        loopStarted = true;
        requestAnimationFrame(GamepadLoop);
    }

    console.log("Gamepad" + e.gamepad.index + " has connected");
});


window.addEventListener("gamepaddisconnected", (e) => {
    !!window.rerender ? window.rerender() : console.log(0);
});

function GamepadLoop() {
    loopStarted = true;

    for (var gamepad of navigator.getGamepads()) {
        if (!gamepad || !gamepad.connected) continue;
        SendControlPacket(gamepad);
    }
  
    requestAnimationFrame(GamepadLoop);
}

function SendControlPacket(gamepad) {
    if (window.serverStatus.InputModuleType != "JSInputModule") { return; }

    var buffer = new ArrayBuffer(20);
    var packet = new DataView(buffer);

    packet.setUint8(0, 255);
    packet.setInt16(1, (gamepad.axes[0] * 32767), true);
    packet.setInt16(3, (gamepad.axes[1] * 32767), true);
    packet.setInt16(5, (gamepad.axes[2] * 32767), true);
    packet.setInt16(7, (gamepad.axes[3] * 32767), true);
    packet.setInt16(9, (gamepad.buttons[6].value * 32767), true);
    packet.setInt16(11, (gamepad.buttons[7].value * 32767), true);

    var buttonGroup1 = [false, false, false, false, false, false, false, false];
    var buttonGroup2 = [false, false, false, false, false, false, false, false];

    for (var i = 0; i < 6; i++) {
        buttonGroup1[i] = gamepad.buttons[i].pressed;
    }
    buttonGroup1[6] = gamepad.buttons[9].pressed; // Menu button
    buttonGroup1[7] = gamepad.buttons[8].pressed; // View button

    for (var i = 10; i < 17; i++) {
        buttonGroup2[i] = gamepad.buttons[i].pressed;
    }

    packet.setUint8(13, parseInt((buttonGroup1.map((v) => !!v ? "1" : "0").join("")), 2)); // Converts to a bitmask
    packet.setUint8(14, parseInt((buttonGroup2.map((v) => !!v ? "1" : "0").join("")), 2));

    if (!!igniteView.commandBridge.forwardInput) {
        igniteView.commandBridge.forwardInput(ToBase64(new Uint8Array(buffer)));
    }
}

function ToBase64(u8) {
    return btoa(String.fromCharCode.apply(null, u8));
}