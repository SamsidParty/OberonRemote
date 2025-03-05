try {
    var socket = new WebSocket(`ws://${window.location.hostname}:${parseInt(window.location.port) + 1}`); // Socket lives at current port + 1

    socket.addEventListener("open", (event) => {
        
    });
}
catch {
    var socket = {
        readyState: -100
    }
}

export function SendControlPacket(gamepad) {
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

    for (var i = 10; i < 16; i++) {
        buttonGroup2[i] = gamepad.buttons[i].pressed;
    }

    packet.setUint8(13, parseInt((buttonGroup1.map((v) => !!v ? "1" : "0").join("")), 2)); // Converts to a bitmask
    packet.setUint8(14, parseInt((buttonGroup2.map((v) => !!v ? "1" : "0").join("")), 2));

    if (socket.readyState == socket.OPEN) {
        socket.send(new Uint8Array(buffer));
    }
}