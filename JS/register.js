document.addEventListener("DOMContentLoaded", function () {
    let db;

    // Funkcija za otvaranje baze podataka
    function openDatabase() {
        return new Promise((resolve, reject) => {
            const request = indexedDB.open("PRA", 1); // "PRA" je naziv baze

            request.onupgradeneeded = function (event) {
                db = event.target.result;

                // Kreiraj objekt store (tablica) ako ne postoji
                if (!db.objectStoreNames.contains("users")) {
                    const objectStore = db.createObjectStore("users", {
                        keyPath: "id",
                    });
                    objectStore.createIndex("username", "username", {
                        unique: true,
                    });
                }
            };

            request.onsuccess = function (event) {
                db = event.target.result;
                resolve(db);
            };

            request.onerror = function (event) {
                console.error("Database error:", event.target.errorCode);
                reject(event.target.errorCode);
            };
        });
    }

    // Otvorimo bazu podataka
    openDatabase().catch((error) => {
        console.error("Failed to open database:", error);
    });

    const btnRegister = document.querySelector("#btn-register");
    if (!btnRegister) {
        console.error("Register button not found.");
        return;
    }

    const username = document.querySelector("#exampleInputUsername1");
    const password = document.querySelector("#exampleInputPassword1");
    const firstName = document.querySelector("#exampleInputFirstName1");
    const email = document.querySelector("#exampleInputEmail1");
    const lastName = document.querySelector("#exampleInputLastName1");

    btnRegister.addEventListener("click", function (event) {
        event.preventDefault();

        const usernameValue = username.value;
        const passwordValue = password.value;
        const firstNameValue = firstName.value;
        const emailValue = email.value;
        const lastNameValue = lastName.value;

        if (
            !usernameValue ||
            !passwordValue ||
            !firstNameValue ||
            !emailValue ||
            !lastNameValue
        ) {
            alert("Please fill all fields.");
            return;
        }

        const registerData = {
            username: usernameValue,
            password: passwordValue,
            firstName: firstNameValue,
            email: emailValue,
            lastName: lastNameValue,
        };

        console.log("register data:", registerData);

        fetch("http://localhost:5103/api/User/register", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify(registerData),
        })
            .then((response) => {
                if (!response.ok) {
                    throw new Error("Registration failed.");
                }
                return response.json(); // Pretpostavljamo da API vraća JSON objekt
            })
            .then((data) => {
                // Provjerite sadržaj data
                console.log("API Response Data:", data);
                const { id, username, firstName, lastName, email, token } = data;

                // Pohranjivanje korisnika u IndexedDB
                if (db) {
                    const transaction = db.transaction(["users"], "readwrite");
                    const objectStore = transaction.objectStore("users");
                    const userData = {
                        id,
                        username,
                        firstName,
                        lastName,
                        email,
                        password: passwordValue // Osigurajte da pohranjujete hashirane lozinke ako koristite hashing
                    };

                    const request = objectStore.put(userData);

                    request.onsuccess = function () {
                        console.log("User added to IndexedDB.");
                    };

                    request.onerror = function (event) {
                        console.error("Transaction error:", event.target.errorCode);
                    };
                }

                // Spremite token i userId u localStorage
                localStorage.setItem("token", token);
                localStorage.setItem("userId", id);

                window.location.href = "../LoggedInUser/CreateKahootQuiz.html";
            })
            .catch((error) => {
                console.error("Error:", error);
                alert("Registration failed. Please try again.");
            });
    });
});
