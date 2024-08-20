document.addEventListener("DOMContentLoaded", function () {
    console.log("DOM ready!");

    const quizName = document.getElementById("title");
    const quizDescription = document.getElementById("quizDescription");

    const quizPanel = document.querySelector(".quiz-panel");
    const questionPanel = document.querySelector(".question-panel");

    const token = localStorage.getItem("token");
    const userId = localStorage.getItem("userId");

    const btnAddAnotherQuestion = document.getElementById("btnAddAnotherQuestion");
    const btnSave = document.getElementById("btnSave");

    const addQuestionForm = document.getElementById('addQuestionForm')


    if (!token || !userId) {
        alert("User not authenticated. Please log in.");
        window.location.href = "../AnonymousUser/Login.html";
        return;
    }

    function clearForm() {
        quizName.value = "";
        quizDescription.value = "";
        quizData.questions = [];
        document.getElementById('addQuestionForm').reset();
        updateSaveBtnState();
    }

    const answers = [
        {
            text: document.getElementById("answer1"),
            correct: document.getElementById("chbxAnswer1"),
        },
        {
            text: document.getElementById("answer2"),
            correct: document.getElementById("chbxAnswer2"),
        },
        {
            text: document.getElementById("answer3"),
            correct: document.getElementById("chbxAnswer3"),
        },
        {
            text: document.getElementById("answer4"),
            correct: document.getElementById("chbxAnswer4"),
        },
    ];

    function updateSaveBtnState() {
        const questionCount = quizData.questions.length;
        btnSave.disabled = questionCount < 3 || questionCount > 20;
    }

    btnAddAnotherQuestion.addEventListener('click', function() {
        const questionText = document.getElementById('questionText').value;
        const questionTime = parseInt(document.getElementById('questionTime').value, 10);
        const questionMaxPoints = parseInt(document.getElementById('questionMaxPoints').value, 10);

       const questionAnswers = answers.map((answer) => ({
            answerText: answer.text.value,
            correct: answer.correct.checked,
       }));
       
        if(!validateAnswers(questionAnswers)){
            alert('You must select just one correct asnswer for each question!');
            return;
        }
        if(!validateQuestion(questionTime, questionMaxPoints)){
            return;
        }
    
        quizData.questions.push({
            questionId: 0,
            questionText: questionText,
            questionPosition: quizData.questions.length + 1,
            questionTime: questionTime,
            questionMaxPoints: questionMaxPoints,
            answers: questionAnswers,
        });

        document.getElementById('addQuestionForm').reset();
        console.log('question added successifully!');
        updateSaveBtnState();


        if(quizData.questions.length >= 3){
            alert('You can now save the quiz!')
        } 
    });

    function validateQuestion(questionTime, questionMaxPoints){
        if(questionTime < 15 || questionTime >= 60){
            alert("Question time must be between 15 and 60 sec!");
            return false;
        }
        if(questionMaxPoints < 1 || questionMaxPoints >= 10  ){
            alert("Max points must be between 1 and 10!");
            return false;
        }
        return true;
    }
    
    function validateAnswers(answers){
        const correctAnswers = answers.filter(answer => answer.correct);
        return correctAnswers.length === 1;
    }

    function displayQuizzes(quizzes) {
        quizPanel.innerHTML = ""; // Clear previous quizzes
    
        quizzes.forEach((quiz) => {
            const quizDiv = document.createElement("div");
            quizDiv.className = "quiz-item border p-3 mb-3";
    
            const quizTitle = document.createElement("h3");
            quizTitle.textContent = quiz.title;
            quizDiv.appendChild(quizTitle);
    
            // Show questions button
            const showQuestionBtn = document.createElement("button");
            showQuestionBtn.textContent = "Show Questions";
            showQuestionBtn.className = "btn btn-show-question mr-2";
            showQuestionBtn.addEventListener("click", () => {
                fetchQuizDetails(quiz.quizId);
            });
            quizDiv.appendChild(showQuestionBtn);
    
            const playBtn = document.createElement('button');
            playBtn.textContent = "Play quiz";
            playBtn.className = "btn btn-play mr-2";
            playBtn.addEventListener("click", () => {
                window.location.href = "../LoggedInUser/KahootGamePin.html";
            });
            quizDiv.appendChild(playBtn);
    
            // Update button
            const updateBtn = document.createElement("button");
            updateBtn.textContent = "Update";
            updateBtn.className = "btn btn-update mr-2";
            updateBtn.addEventListener("click", () => {
                handleUpdateQuiz(quiz);
            });
            quizDiv.appendChild(updateBtn);
    
            // Delete button
            const deleteBtn = document.createElement("button");
            deleteBtn.textContent = "Delete";
            deleteBtn.className = "btn btn-delete mr-2";
            deleteBtn.addEventListener("click", () => {
                handleDeleteQuiz(quiz.quizId);
            });
            quizDiv.appendChild(deleteBtn);
    
            quizPanel.appendChild(quizDiv);
        });
    }
    

    document.getElementById('btnNext').addEventListener('click', function() {

        if (!quizName.value.trim() || !quizDescription.value.trim()) {
            alert('Please enter quiz name and description');
            btnNext.style.display = 'none';
            return;
        }
    
        quizInfoForm.title = quizName.value;
        quizInfoForm.description = quizDescription.value;
        quizInfoForm.authorId = parseInt(userId, 10);
        
        document.getElementById('quizInfoForm').style.display = 'none';
        addQuestionForm.style.display = 'block';
    });

    let quizData = {
        quizId: 0,
        title: "",
        description: "",
        authorId: parseInt(userId, 10),
        questions: []
    };


    addQuestionForm.addEventListener('submit', function(event){
        event.preventDefault();

        const questionText = document.getElementById('questionText').value;
        const questionTime = document.getElementById('questionTime').value;
        const questionMaxPoints = document.getElementById('questionMaxPoints').value;

        if(!validateAnswers(questionAnswers)){
            alert('You must select just one correct asnswer for each question!');
            return;
        }
        if(!validateQuestion(questionTime, questionMaxPoints)){
            return;
        }

        quizData.questions.push({
            questionId: 0, 
            questionText: questionText,
            questionPosition: quizData.questions.length + 1, // Position of question in quiz
            questionTime: questionTime,
            questionMaxPoints: questionMaxPoints,
            answers: answers.map((answer) => ({
                answerId: 0, // Postavljamo na 0 jer kreiramo novi odgovor
                answerText: answer.text.value,
                correct: answer.correct.checked
            }))
        });

        //ocisti formu
        document.getElementById('addQuestionForm').reset();
        updateSaveBtnState();
    })


    function handleUpdateQuiz(quiz) {
        const updatedTitle = prompt("Enter new title: ", quiz.title);
        const updatedDescription = prompt("Enter new description: ", quiz.description);

        if (!updatedTitle || !updatedDescription) {
            alert("Title and description are required.");
            return;
        }

        const updateQuizData = {
            quizId: quiz.quizId,
            title: updatedTitle,
            description: updatedDescription,
            authorId: quiz.authorId,
            questions: quiz.questions, // Keeping existing questions
        };

        fetch(`http://localhost:5103/api/quiz/${quiz.quizId}`, {
            method: "PUT",
            headers: {
                "Content-Type": "application/json",
                Authorization: `Bearer ${token}`,
            },
            body: JSON.stringify(updateQuizData),
        })
            .then(response => {
                if (!response.ok) {
                    return response.text().then(text => {
                        console.error("Server error response:", text);
                        throw new Error("Error updating quiz. Please check server response.");
                    });
                }
                return response.json();
            })
            .then(data => {
                console.log("Quiz updated successfully:", data);
                alert("Quiz updated successfully!");
                fetchQuiz();
            })
            .catch(error => {
                console.error("Error:", error.message);
                alert("Error updating quiz. Please try again.");
            });
    }


    function handleUpdateQuestion(quizId, question) {
        const updatedQuestionText = prompt("Enter new question text: ", question.questionText);
        const updatedQuestionTime = prompt("Enter new question time (15-60 sec): ", question.questionTime);
        const updatedQuestionMaxPoints = prompt("Enter new max points (1-10): ", question.questionMaxPoints);
    
        if (!updatedQuestionText || !validateQuestion(updatedQuestionTime, updatedQuestionMaxPoints)) {
            alert("Invalid input. Please try again.");
            return;
        }
    
        const updatedAnswers = question.answers.map((answer, index) => {
            const updatedAnswerText = prompt(`Enter text for answer ${index + 1}:`, answer.answerText);
            const updatedCorrectAnswer = confirm(`Is this the correct answer? (current: ${answer.correct})`);
            return {
                answerId: answer.answerId,
                answerText: updatedAnswerText,
                correct: updatedCorrectAnswer,
            };
        });
    
        const updatedQuestion = {
            questionId: question.questionId,
            questionText: updatedQuestionText,
            questionTime: parseInt(updatedQuestionTime, 10),
            questionMaxPoints: parseInt(updatedQuestionMaxPoints, 10),
            answers: updatedAnswers,
        };
    
        updateQuestionInQuiz(quizId, question.questionId, updatedQuestion);
    }
    

    function handleDeleteQuiz(quizId) {
        if (confirm("Are you sure you want to delete this quiz?")) {
            fetch(`http://localhost:5103/api/quiz/${quizId}`, {
                method: "DELETE",
                headers: {
                    "Content-Type": "application/json",
                    Authorization: `Bearer ${token}`,
                },
            })
                .then(response => {
                    if (!response.ok) {
                        return response.text().then(text => {
                            console.error("Server error response:", text);
                            throw new Error("Error deleting quiz. Please check server response.");
                        });
                    }
                    return response.json();
                })
                .then(data => {
                    console.log("Quiz deleted successfully:", data);
                    alert("Quiz deleted successfully!");
                    fetchQuiz();
                })
                .catch(error => {
                    console.error("Error:", error.message);
                    alert("Error deleting quiz. Please try again.");
                });
        }
    }


    function fetchQuiz() {
        fetch(`http://localhost:5103/api/quiz/details/${userId}`, {
            method: "GET",
            headers: {
                Authorization: `Bearer ${token}`,
            },
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error("Response error: " + response.statusText);
                }
                return response.json();
            })
            .then(data => {
                console.log(data);
                if (Array.isArray(data) && data.length > 0) {
                    displayQuizzes(data);
                } else {
                    alert("No quizzes found or unexpected response format.");
                }
            })
            .catch(error => {
                console.error("Fetch error:", error);
                alert("An error occurred while fetching quizzes.");
            });
    }


    function fetchQuizDetails(quizId) {
        fetch(`http://localhost:5103/api/quiz/${quizId}`, {
            method: "GET",
            headers: {
                Authorization: `Bearer ${token}`,
            },
        })
        .then(response => {
            if (!response.ok) {
                throw new Error("Error fetching quiz details");
            }
            return response.json();
        })
        .then(data => {
            quizData = data; // Store quiz data
            displayQuestions(data); // Display questions
        })
        .catch(error => {
            console.error("Error:", error.message);
            alert("Error fetching quiz details. Please try again.");
        });
    }
    
    function updateQuestionInQuiz(quizId, questionId, updatedQuestion) {

        
        const questionIndex = quizData.questions.findIndex(q => q.questionId === questionId);
        if (questionIndex === -1) {
            alert("Question not found in quiz.");
            return;
        }

        quizData.questions[questionIndex] = {
            ...quizData.questions[questionIndex],//primjer. // { questionId: 1, questionText: "Old text", questionTime: 30 }
            ...updatedQuestion //primjer. { questionText: "New text", questionMaxPoints: 10 }
            //primjer finalnog izgleda objeka je
        };
    
       
        fetch(`http://localhost:5103/api/quiz`, {
            method: "PUT",
            headers: {
                "Content-Type": "application/json",
                Authorization: `Bearer ${token}`,
            },
            body: JSON.stringify(quizData),
        })
        .then(response => {
            if (!response.ok) {
                return response.text().then(text => {
                    console.error("Server error response:", text);
                    throw new Error("Error updating quiz. Please check server response.");
                });
            }
            return response.json();
        })
        .then(data => {
            console.log("Quiz updated successfully with the updated question:", data);
            alert("Question updated successfully!");
            fetchQuizDetails(quizId); 
        })
        .catch(error => {
            console.error("Error:", error.message);
            alert("Error updating question. Please try again.");
        });
    }

    function displayQuestions(quiz) {
        console.log("Displaying questions for quiz:", quiz);
        console.log("Questions data:", quiz.questions);
    
        if (!quiz.questions || !Array.isArray(quiz.questions)) {
            console.error("Questions data is not an array:", quiz.questions);
            alert("Error: Questions data is not available.");
            return;
        }
    
        questionPanel.innerHTML = ""; // Očistite prethodni sadržaj
    
        quiz.questions.forEach((question, index) => {
            const questionDiv = document.createElement("div");
            questionDiv.className = "question-item border p-3 mb-3";
    
            const questionTextElement = document.createElement("h4");
            questionTextElement.textContent = `Question ${index + 1}: ${question.questionText}`;
            questionDiv.appendChild(questionTextElement);
    
            const answerList = document.createElement("ul");
    
            question.answers.forEach((answer) => {
                const answerItem = document.createElement("li");
                answerItem.textContent = `${answer.answerText} - ${answer.correct ? "Correct" : "Incorrect"}`;
                answerList.appendChild(answerItem);
            });
            questionDiv.appendChild(answerList);
    
            const editButton = document.createElement("button");
            editButton.textContent = "Edit";
            editButton.className = "btn btn-update";
            editButton.onclick = () => editQuestion(quiz.quizId, index); // ispravan indeks
    
            questionDiv.appendChild(editButton);
    
            questionPanel.appendChild(questionDiv);
        });
    
        questionPanel.style.display = "block"; // Prikazivanje panela s pitanjima
    }
    
    

    function editQuestion(quizId, questionIndex) {
        const quiz = quizData; // `quizData` mora biti ažuriran
        console.log("Editing question for quiz ID:", quizId);
        console.log("Question Index:", questionIndex);
        console.log("Quiz Data:", quiz);
    
        if (!quiz || !Array.isArray(quiz.questions) || questionIndex >= quiz.questions.length || questionIndex < 0) {
            console.error("Question not found at index:", questionIndex);
            alert("Error: Question not found.");
            return;
        }
    
        const question = quiz.questions[questionIndex];
        handleUpdateQuestion(quizId, question); 
    }
    

    btnSave.addEventListener("click", function (event) {
        event.preventDefault();
    
        if (!quizName.value.trim() || !quizDescription.value.trim()) {
            alert("Quiz name and description are required.");
            return;
        }

        if (quizData.questions.length < 3 || quizData.questions.length > 20) {
            alert("Quiz must have between 3 and 20 questions.");
            return;
        }
    
        quizData.title = quizName.value.trim();
        quizData.description = quizDescription.value.trim();
        quizData.authorId = parseInt(userId, 10); // authorId iz localStorage
    
        console.log("Quiz data to send:", JSON.stringify(quizData, null, 2));
    
        fetch("http://localhost:5103/api/quiz", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                Authorization: `Bearer ${token}`,
            },
            body: JSON.stringify(quizData),
        })
        .then(response => {
            if (!response.ok) {
                return response.text().then(text => {
                    console.error("Server error response:", text);
                    throw new Error("Error saving quiz. Please check server response.");
                });
            }
            return response.json();
        })
        .then(data => {
            console.log("Success:", data);
            alert("Quiz saved successfully!");
            clearForm();
            fetchQuiz(); 
        })
        .catch(error => {
            console.error("Error:", error.message);
            alert("Error saving quiz. Please try again.");
        });
    });

    document.addEventListener('DOMContentLoaded', () => {
        loadQuizzes();
    
        document.getElementById('quizForm').addEventListener('submit', function(event) {
            event.preventDefault();
    
            const title = document.getElementById('quizTitle').value;
            const description = document.getElementById('quizDescription').value;
    
            const newQuiz = {
                title: title,
                description: description,
                questions: []
            };
    
            saveQuiz(newQuiz);
    
            loadQuizzes();
            
            document.getElementById('createQuizPanel').style.display = 'none';
            document.getElementById('quizPanel').style.display = 'block';
        });
    
        
        document.getElementById('btnCreateNewQuiz').addEventListener('click', () => {
            document.getElementById('createQuizPanel').style.display = 'block';
            document.getElementById('quizPanel').style.display = 'none';
        });
    });
    
    function saveQuiz(quiz) {
        const quizzes = JSON.parse(localStorage.getItem('quizzes')) || [];
        quizzes.push(quiz);
        localStorage.setItem('quizzes', JSON.stringify(quizzes));
    }
    
    function loadQuizzes() {
        const quizzes = JSON.parse(localStorage.getItem('quizzes')) || [];
        const quizList = document.getElementById('quizList');
    
        quizList.innerHTML = ''; 
    
        quizzes.forEach((quiz, index) => {
            const listItem = document.createElement('li');
            listItem.className = 'list-group-item';
            listItem.textContent = quiz.title;
            listItem.addEventListener('click', () => {
                displayQuizDetails(quiz);
            });
            quizList.appendChild(listItem);
        });
    }
    
    
    fetchQuiz()

});
