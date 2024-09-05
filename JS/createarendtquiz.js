document.addEventListener("DOMContentLoaded", function () {
    console.log("DOM ready!");
 
    const quizName = document.getElementById("title");
    const quizDescription = document.getElementById("quizDescription");
 
    const quizPanel = document.querySelector(".quiz-panel");
    const questionPanel = document.querySelector(".question-panel");
 
    const token = localStorage.getItem("token");
 
    const btnAddAnotherQuestion = document.getElementById("btnAddAnotherQuestion");
    const btnSave = document.getElementById("btnSave");
 
    const addQuestionForm = document.getElementById('addQuestionForm');
 
    // Authentication check
    if (!token) {
        alert("User not authenticated. Please log in.");
        window.location.href = "../AnonymousUser/Login.html";
        return;
    }
 
    // Dekodiraj JWT
    const decodedToken = jwt_decode(token);
    console.log("Decoded JWT:", decodedToken);
 
    const { Username, sub, Id } = decodedToken;
 
    localStorage.setItem('username', Username );

    localStorage.setItem('authorId', Id ); 

    let quizData = {
        quizId: 0,
        title: "",
        description: "",
        authorId: parseInt(Id, 10),
        questions: []
    }; 

    function clearForm() {
        quizName.value = "";
        quizDescription.value = "";
        quizData.questions = [];
        addQuestionForm.reset();
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
 
    btnAddAnotherQuestion.addEventListener('click', function () {
        const questionText = document.getElementById('questionText').value;
        const questionTime = parseInt(document.getElementById('questionTime').value, 10);
        const questionMaxPoints = parseInt(document.getElementById('questionMaxPoints').value, 10);
 
        const questionAnswers = answers.map((answer) => ({
            answerText: answer.text.value,
            correct: answer.correct.checked,
        }));
 
        if (!validateAnswers(questionAnswers)) {
            alert('You must select just one correct answer for each question!');
            return;
        }
        if (!validateQuestion(questionTime, questionMaxPoints)) {
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
 
        addQuestionForm.reset();
        console.log('Question added successfully!');
        updateSaveBtnState();
 
        if (quizData.questions.length >= 3) {
            alert('You can now save the quiz!');
        }
    });
 
    function validateQuestion(questionTime, questionMaxPoints) {
        if (questionTime < 15 || questionTime >= 60) {
            alert("Question time must be between 15 and 60 sec!");
            return false;
        }
        if (questionMaxPoints < 1 || questionMaxPoints >= 1000) {
            alert("Max points must be between 1 and 1000!");
            return false;
        }
        return true;
    }
 
    function validateAnswers(answers) {
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
                localStorage.setItem("quizId", quiz.quizId); 
                
                fetch(`http://193.198.217.170:8080/api/quiz/${quiz.quizid}`, {
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
                    const questionsLength = quiz.questions.length;
                    localStorage.setItem('questionsLength', questionsLength);
                    console.log(questionsLength);             
                }) 
                .catch(error => {
                    console.error("Error:", error.message);
                }); 

                window.location.href = "../LoggedInUser(SessionRequired)/ArendtGamePin.html"; 
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
 
    document.getElementById('btnNext').addEventListener('click', function () {
        if (!quizName.value.trim() || !quizDescription.value.trim()) {
            alert('Please enter quiz name and description');
            return;
        }
 
        quizData.title = quizName.value;
        quizData.description = quizDescription.value;
        quizData.authorId = parseInt(Id, 10);
 
        document.getElementById('quizInfoForm').style.display = 'none';
        addQuestionForm.style.display = 'block';
    });
 
    addQuestionForm.addEventListener('submit', function (event) {
        event.preventDefault();
 
        const questionText = document.getElementById('questionText').value;
        const questionTime = parseInt(document.getElementById('questionTime').value, 10);
        const questionMaxPoints = parseInt(document.getElementById('questionMaxPoints').value, 10);
 
        const questionAnswers = answers.map((answer) => ({
            answerText: answer.text.value,
            correct: answer.correct.checked,
        }));
 
        if (!validateAnswers(questionAnswers)) {
            alert('You must select just one correct answer for each question!');
            return;
        }
        if (!validateQuestion(questionTime, questionMaxPoints)) {
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
 
        addQuestionForm.reset();
        updateSaveBtnState();
    });
 
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
            questions: quiz.questions,
        };
 
        fetch(`http://193.198.217.170:8080/api/quiz/${quiz.quizId}`, {
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
        const updatedQuestionTime = prompt("Enter new question time (15-60 seconds): ", question.questionTime);
        const updatedQuestionMaxPoints = prompt("Enter new max points (1-10): ", question.questionMaxPoints);
 
        if (!updatedQuestionText || !updatedQuestionTime || !updatedQuestionMaxPoints) {
            alert("Question text, time, and max points are required.");
            return;
        }
 
        const updateQuestionData = {
            questionId: question.questionId,
            questionText: updatedQuestionText,
            questionTime: parseInt(updatedQuestionTime, 10),
            questionMaxPoints: parseInt(updatedQuestionMaxPoints, 10),
            quizId: quizId,
            answers: question.answers,
        };
 
        fetch(`http://193.198.217.170:8080/api/question/${question.questionId}`, {
            method: "PUT",
            headers: {
                "Content-Type": "application/json",
                Authorization: `Bearer ${token}`,
            },
            body: JSON.stringify(updateQuestionData),
        })
            .then(response => {
                if (!response.ok) {
                    return response.text().then(text => {
                        console.error("Server error response:", text);
                        throw new Error("Error updating question. Please check server response.");
                    });
                }
                return response.json();
            })
            .then(data => {
                console.log("Question updated successfully:", data);
                alert("Question updated successfully!");
                fetchQuizDetails(quizId);
            })
            .catch(error => {
                console.error("Error:", error.message);
                alert("Error updating question. Please try again.");
            });
    }
 
    function handleDeleteQuiz(quizId) {
        if (confirm("Are you sure you want to delete this quiz?")) {
            fetch(`http://193.198.217.170:8080/api/quiz/${quizId}`, {
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
        fetch("http://193.198.217.170:8080/api/quiz", {
            method: "GET",
            headers: {
                Authorization: `Bearer ${token}`,
            },
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error("Error fetching quizzes. Please try again.");
                }
                return response.json();
            })
            .then(data => {
                console.log("Fetched quizzes:", data);
                displayQuizzes(data);
            })
            .catch(error => {
                console.error("Error:", error.message);
            });
    }
 
    function fetchQuizDetails(quizId) {
        fetch(`http://193.198.217.170:8080/api/quiz/${quizId}`, {
            method: "GET",
            headers: {
                Authorization: `Bearer ${token}`,
            },
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error("Error fetching quiz details. Please try again.");
                }
                return response.json();
            })
            .then(quiz => {
                displayQuizQuestions(quiz);
            })
            .catch(error => {
                console.error("Error:", error.message);
            });
    }
 
    function displayQuizQuestions(quiz) {
        questionPanel.innerHTML = "";
        const questionsTitle = document.createElement("h3");
        questionsTitle.textContent = "Questions";
        questionPanel.appendChild(questionsTitle);
 
        quiz.questions.forEach((question, index) => {
            const questionDiv = document.createElement("div");
            questionDiv.className = "question-item border p-3 mb-3";
 
            const questionTitle = document.createElement("h5");
            questionTitle.textContent = `${index + 1}. ${question.questionText}`;
            questionDiv.appendChild(questionTitle);
 
            const questionInfo = document.createElement("p");
            questionInfo.textContent = `Time: ${question.questionTime} sec | Max Points: ${question.questionMaxPoints}`;
            questionDiv.appendChild(questionInfo);
 
            const updateQuestionBtn = document.createElement("button");
            updateQuestionBtn.textContent = "Update Question";
            updateQuestionBtn.className = "btn btn-update-question mr-2";
            updateQuestionBtn.addEventListener("click", () => {
                handleUpdateQuestion(quiz.quizId, question);
            });
            questionDiv.appendChild(updateQuestionBtn);
 
            questionPanel.appendChild(questionDiv);
        });
    }
 
    btnSave.addEventListener("click", function () {
        if (quizData.questions.length < 3 || quizData.questions.length > 20) {
            alert("Quiz must have at least 3 and at most 20 questions!");
            return;
        }
 
        fetch("http://193.198.217.170:8080/api/quiz", {
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
                console.log("Quiz saved successfully:", data);
                alert("Quiz saved successfully!");
                clearForm();
                fetchQuiz();
            })
            .catch(error => {
                console.error("Error:", error.message);
                alert("Error saving quiz. Please try again.");
            });
    });
 
    // Initial fetch to load quizzes
    fetchQuiz();
}); 