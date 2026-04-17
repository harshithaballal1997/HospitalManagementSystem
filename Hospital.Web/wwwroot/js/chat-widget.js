document.addEventListener("DOMContentLoaded", function () {
    const chatToggle = document.getElementById("aiChatToggle");
    const chatWindow = document.getElementById("aiChatWindow");
    const chatClose = document.getElementById("aiChatClose");
    const chatInput = document.getElementById("aiChatInput");
    const chatSend = document.getElementById("aiChatSend");
    const chatMessages = document.getElementById("aiChatMessages");

    let isTyping = false;

    // Toggle Chat Window
    if (chatToggle && chatWindow && chatClose) {
        chatToggle.addEventListener("click", () => {
            chatWindow.style.display = chatWindow.style.display === "none" || chatWindow.style.display === "" ? "flex" : "none";
            if (chatWindow.style.display === "flex") {
                chatInput.focus();
            }
        });

        chatClose.addEventListener("click", () => {
            chatWindow.style.display = "none";
        });
    }

    // SignalR Setup
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/chatHub")
        .withAutomaticReconnect()
        .build();

    connection.on("ReceiveMessage", function (user, message) {
        // Remove typing indicator if exists
        const typingEl = document.getElementById("aiTypingIndicator");
        if (typingEl) typingEl.remove();
        isTyping = false;

        const msgDiv = document.createElement("div");
        msgDiv.className = user === "You" ? "chat-msg user-msg" : "chat-msg bot-msg";
        
        let formattedMessage = message; // convert markdown bold or newlines to HTML
        formattedMessage = formattedMessage.replace(/\*\*(.*?)\*\*/g, "<strong>$1</strong>");
        formattedMessage = formattedMessage.replace(/\n/g, "<br>");

        msgDiv.innerHTML = `<span class="chat-sender">${user}:</span> ${formattedMessage}`;
        chatMessages.appendChild(msgDiv);
        chatMessages.scrollTop = chatMessages.scrollHeight;
    });

    connection.start().catch(function (err) {
        return console.error(err.toString());
    });

    function showTyping(message = "") {
        if (isTyping) return;
        isTyping = true;
        const typingDiv = document.createElement("div");
        typingDiv.id = "aiTypingIndicator";
        typingDiv.className = "chat-msg bot-msg typing";
        
        let loadingText = "";
        if (message.toLowerCase().includes("summar") || message.toLowerCase().includes("history")) {
            loadingText = "<i class='fas fa-microscope me-2'></i> Hospital AI is generating your clinical summary...";
        }

        typingDiv.innerHTML = `${loadingText}<div class="d-flex gap-1 ms-2"><span class="dot"></span><span class="dot"></span><span class="dot"></span></div>`;
        chatMessages.appendChild(typingDiv);
        chatMessages.scrollTop = chatMessages.scrollHeight;
    }

    function sendMessage() {
        const message = chatInput.value.trim();
        if (message) {
            connection.invoke("SendMessage", message).catch(function (err) {
                return console.error(err.toString());
            });
            const lastMessage = message;
            chatInput.value = "";
            showTyping(lastMessage);
        }
    }

    if (chatSend && chatInput) {
        chatSend.addEventListener("click", sendMessage);
        chatInput.addEventListener("keypress", function (e) {
            if (e.key === "Enter") {
                e.preventDefault();
                sendMessage();
            }
        });
    }
});
