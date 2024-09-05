document.addEventListener('DOMContentLoaded', () => {
    const token = localStorage.getItem('token');
    const quizId = localStorage.getItem('quizId');
    const username = localStorage.getItem('username');
    const questionsLength = localStorage.getItem('questionsLength'); 
    let sessionId = null; 
    let previousPlayerList = [];  // Prethodna lista korisnika
    let playerCount = 0; 
    var parsedResponse = null;  

    const webSocket = new WebSocket(`ws://193.198.217.170:8181/?Username=${username}&QuizId=${quizId}`);
 
    webSocket.addEventListener('open', function (event) {
        console.log('WebSocket connection established.');
    }); 
    
    webSocket.addEventListener('message', function (event) {
        console.log('Message received from server:', event);
        
        const receivedData = event.data.trim();

        let jsonResponse = JSON.parse(receivedData); 

        let keys = Object.keys(jsonResponse); 
        
        const isSixDigitSessionId = /^[0-9]{6}$/.test(receivedData);
 
        let storedSessionId = localStorage.getItem('sessionId');
        const receivedSessionId = receivedData;

        function isUsernameFirstKey(data) { 
            if (Array.isArray(data) && data.length > 0) {

                const firstObject = data[0];
                
                const keys = Object.keys(firstObject);
                
                return keys.length > 0 && keys[0] === 'Username';
            }
            return false;
        }
        
        const isFirstKey = isUsernameFirstKey(jsonResponse); 
        
        if (!storedSessionId || storedSessionId !== receivedSessionId && isSixDigitSessionId) {
            console.log('Storing new session ID:', receivedSessionId);
            localStorage.setItem('sessionId', receivedSessionId);
            sessionId = receivedSessionId;
 
            const sessionCode = document.getElementById('session-code');
            if (sessionCode) {
                sessionCode.innerHTML = `<p>${sessionId}</p>`;
            }
        } else if (storedSessionId && !isSixDigitSessionId && receivedData.length < 100 && !isFirstKey) {
            console.log('Session ID is already stored:', storedSessionId);
 
            const playerDataArray = JSON.parse(receivedData);
 
            if (Array.isArray(playerDataArray)) {
                const currentPlayerList = playerDataArray;
                updatePlayerList(currentPlayerList);
            }
        } else if (keys[0] === "QuestionId") {   
            console.log("Quiz received from server", receivedData);

            parsedResponse = JSON.parse(receivedData); 

            document.getElementById("question").textContent = parsedResponse.QuestionText; 

            const answerPanels = document.querySelectorAll('.answer-panel');
    
            const answers = parsedResponse.Answers;
    
            for (let i = 0; i < answerPanels.length; i++) {
                if (i < answers.length) { 
    
                    let panel = answerPanels[i];
                    let answer = answers[i];
    
                    let textElement = panel.querySelector('.answer-text');
                    if (textElement) {
                        textElement.textContent = ` ${answer.AnswerText}`;
                    } 
                }
            } 

            var questionTime = 0;  
            
            questionTime = parsedResponse.QuestionTime; 
            
            startTimer(questionTime); 

        } 
     else if (isFirstKey) {   

        console.log("Leaderboard received from server", receivedData);

    try {
        const leaderboardData = JSON.parse(receivedData);

        const overlayPanel = document.getElementById('overlay');
        const leaderboardContainer = overlayPanel.querySelector('#leaderboard-container');

        if (!leaderboardContainer) {
            console.error("Leaderboard container not found.");
            return;
        }

        leaderboardContainer.innerHTML = '';

        function createPlayerPanel(username, score, rank) {
            let panelClass = '';
            switch (rank) {
                case 1: panelClass = 'gold'; break;
                case 2: panelClass = 'silver'; break;
                case 3: panelClass = 'bronze'; break;
                default: panelClass = ''; break;
            }

            return `
                <div class="player-panel ${panelClass}">
                    <span class="player-name">${rank} player: ${username}</span>
                    <span class="player-points">${score} points</span>
                </div>
            `;
        }

        function updateLeaderboard(data) {
            if (!Array.isArray(data)) {
                console.error("Invalid leaderboard data format.");
                return;
            }

            data.sort((a, b) => b.Score - a.Score);

            data.forEach((player, index) => {
                const playerPanel = createPlayerPanel(player.Username, player.Score, index + 1);
                leaderboardContainer.innerHTML += playerPanel;
            });

            overlayPanel.style.display = 'block';
        } 

        updateLeaderboard(leaderboardData);

    } catch (error) {
        console.error("Error parsing leaderboard data:", error);
    } 
}  

        else { 
            console.log("Some unknown error happened"); 
        } 
    }); 

    
    webSocket.onclose = () => {
        console.log('WebSocket connection closed.');
    
        if (lastMessage) {
            console.log('Last message before closing:', lastMessage);
            // Optionally, display the last message on the UI
            // document.getElementById('yourMessageDisplayElement').innerText = lastMessage;
        } else {
            console.log('No messages received before closing.');
        }
    };
 
    webSocket.onerror = (error) => {
        console.error('WebSocket error:', error);
    };
    
    fetch(`http://193.198.217.170:8080/api/quiz/${quizId}`, {
        method: "GET",
        headers: {
            Authorization: `Bearer ${token}`,
        },
    })
    .then(response => {
        if (!response.ok) {
            throw new Error("Error fetching quiz details.");
        }
        return response.json();
    })
    .then(quiz => {
        console.log(quiz);
        const quizLength = quiz.questions.length;
        localStorage.setItem('questionsLength', quizLength); 
    }) 
    .catch(error => {
        console.error("Error:", error.message);
    }); 
    
    function updatePlayerList(currentPlayerList) {
 
        const previousPlayerSet = new Set(previousPlayerList);
        const currentPlayerSet = new Set(currentPlayerList);
 
        currentPlayerList.forEach(player => {
            if (!previousPlayerSet.has(player)) {
                console.log(`Adding new player: ${player}`);
                const playerNameElement = document.createElement('p');
                playerNameElement.textContent = player;
                document.getElementById('playerList').appendChild(playerNameElement);
                playerCount++;
            }
        });
 
        previousPlayerList.forEach(player => {
            if (!currentPlayerSet.has(player)) {
                console.log(`Removing disconnected player: ${player}`);
                const playerListDiv = document.getElementById('playerList');
                const playerElements = playerListDiv.querySelectorAll('p');
                playerElements.forEach(element => {
                    if (element.textContent === player) {
                        playerListDiv.removeChild(element);
                        playerCount--;
                    }
                });
            }
        });
 
        document.getElementById('playerCount').textContent = `Player Count: ${playerCount}`;
 
        previousPlayerList = currentPlayerList;
    } 
    
    const playBtn = document.getElementById('KahootGamePin-playBtn'); 
    const finishPage = document.getElementById("finishPage"); 
    const createKahootPinPage = document.getElementById ("CreateKahootPinPage");
    const gameProgressPage = document.getElementById("GameProgressPage"); 
    const overlayElement = document.getElementById('overlay');
    
    const timeQuestion = localStorage.getItem("questionTime"); 
    
    playBtn.addEventListener("click", function () {
        console.log("PLAY BTN CLICKED");
        createKahootPinPage.style.display = "none";

        gameProgressPage.style.display = "block";

        if (webSocket.readyState === WebSocket.OPEN) {  // Check if the connection is open
            webSocket.send("1");   // Send the number 1
            console.log('Sent number 1');
        } else { 
            console.log('WebSocket is not open.');
        } 
    }); 

    function startTimer(totalTime) {
        // Parse and validate totalTime
        console.log(totalTime); 
        if (isNaN(totalTime) || totalTime <= 0) {
            console.error("Invalid total time value");
            return;
        } 
        
        var timeLeft = totalTime;
        var timerElement = document.getElementById('timer');
        var timerNumberElement = document.getElementById('timer-number');
        var overlayElement = document.getElementById('overlay');
    
        // Set initial timer state
        timerNumberElement.innerHTML = timeLeft;
        timerElement.style.background = `conic-gradient(transparent 0% ${((totalTime - timeLeft) / totalTime) * 360}deg, green ${((totalTime - timeLeft) / totalTime) * 360}deg 360deg)`;
    
        var timerInterval = setInterval(function() {
            if (timeLeft === 0) {              
                var angle = 360; 
                timerElement.style.background = `conic-gradient(transparent 0% ${angle}deg, red ${angle}deg 360deg)`;
                timerNumberElement.innerHTML = "Time!";
                webSocket.send("leaderboard"); 
                gameProgressPage.style.display = "none"; 
                overlayElement.style.display = 'flex'; 
                clearInterval(timerInterval); 
            } else { 
                // Update timer display 
                timerNumberElement.innerHTML = timeLeft;
                var angle = ((totalTime - timeLeft) / totalTime) * 360;
                var color = 'green';
    
                if (timeLeft <= totalTime * 0.25) {
                    color = 'red';
                } else if (timeLeft <= totalTime * 0.50) {
                    color = 'yellow';
                }
    
                timerElement.style.background = `conic-gradient(transparent 0% ${angle}deg, ${color} ${angle}deg 360deg)`;
                timeLeft -= 1;
            }
        }, 1000);
    } 
    
    const nextBtn = document.getElementById('btnLeaderboard'); 

    nextBtn.addEventListener("click", function () { 
       sendNumber(); 
    
        overlayElement.style.display = "none"; 

        gameProgressPage.style.display = "block"; 
    }); 
    
    var numberCount = 2;  
    
    function sendNumber(){ 
        if(webSocket.readyState === WebSocket.OPEN){
            if (numberCount <= questionsLength) 
                { 
                    webSocket.send(String(numberCount)); 
                    numberCount++; 
                } 
            else if (numberCount > questionsLength) 
                { 
                    webSocket.send("close");  
                    console.log("Web socket is closed");  
                }            
        }else{ 
            console.log('WebSocket is  not open'); 
        }  
    } 
}); 
