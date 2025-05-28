let localStream;
let peerConnection;
let callWebSocket;
let roomId;

const configuration = { iceServers: [{ urls: "stun:stun.l.google.com:19302" }] };

// Подключение к комнате
function joinRoom(room) {
    roomId = room;
    callWebSocket = new WebSocket(`ws://localhost:5091/ws/call/${roomId}`);

    callWebSocket.onmessage = async (event) => {
        const message = JSON.parse(event.data);

        if (message.type === "offer") {
            // Если получен offer, создаем answer и отправляем его обратно
            await peerConnection.setRemoteDescription(new RTCSessionDescription({ type: "offer", sdp: message.sdp }));
            const answer = await peerConnection.createAnswer();
            await peerConnection.setLocalDescription(answer);
            callWebSocket.send(JSON.stringify({ type: "answer", sdp: answer.sdp }));
        } else if (message.type === "answer") {
            // Если получен answer, устанавливаем его как удаленное описание
            await peerConnection.setRemoteDescription(new RTCSessionDescription({ type: "answer", sdp: message.sdp }));
        } else if (message.type === "candidate") {
            // Если получен ICE candidate, добавляем его
            await peerConnection.addIceCandidate(new RTCIceCandidate(message.candidate));
        }
    };

    callWebSocket.onopen = () => {
        console.log("Connected to call room:", roomId);
    };

    callWebSocket.onclose = () => {
        console.log("Disconnected from call room:", roomId);
    };
}

// Выход из комнаты
function leaveRoom() {
    if (callWebSocket) {
        callWebSocket.close();
        callWebSocket = null;
        roomId = null;
    }
}

// Начало звонка
async function startCall() {
    if (!callWebSocket || callWebSocket.readyState !== WebSocket.OPEN) {
        alert("Please join a room first!");
        return;
    }

    // Получаем доступ к камере и микрофону
    localStream = await navigator.mediaDevices.getUserMedia({ video: true, audio: true });
    document.getElementById("localVideo").srcObject = localStream;

    // Создаем RTCPeerConnection
    peerConnection = new RTCPeerConnection(configuration);
    peerConnection.onicecandidate = (event) => {
        if (event.candidate) {
            // Отправляем ICE candidate другим участникам комнаты
            callWebSocket.send(JSON.stringify({ type: "candidate", candidate: event.candidate }));
        }
    };

    peerConnection.ontrack = (event) => {
        // Отображаем удаленный поток
        document.getElementById("remoteVideo").srcObject = event.streams[0];
    };

    // Добавляем локальный поток в соединение
    localStream.getTracks().forEach(track => peerConnection.addTrack(track, localStream));

    // Создаем offer и отправляем его другим участникам комнаты
    const offer = await peerConnection.createOffer();
    await peerConnection.setLocalDescription(offer);
    callWebSocket.send(JSON.stringify({ type: "offer", sdp: offer.sdp }));
}

// Демонстрация экрана
async function shareScreen() {
    if (!callWebSocket || callWebSocket.readyState !== WebSocket.OPEN) {
        alert("Please join a room first!");
        return;
    }

    // Захватываем экран и добавляем его в соединение
    const screenStream = await navigator.mediaDevices.getDisplayMedia({ video: true });
    screenStream.getTracks().forEach(track => peerConnection.addTrack(track, screenStream));
    document.getElementById("localVideo").srcObject = screenStream;
}
