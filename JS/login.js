document.addEventListener("DOMContentLoaded", function () {
    let db;

    // Funkcija za otvaranje baze podataka
    function openDatabase() {
        return new Promise((resolve, reject) => {
            const request = indexedDB.open("PRA", 1); // Naziv baze

            request.onupgradeneeded = function (event) {
                const db = event.target.result;

                // Kreiraj objekt store (tablica) ako ne postoji
                if (!db.objectStoreNames.contains("users")) {
                    const objectStore = db.createObjectStore("users", {
                        keyPath: "id",
                    });
                    objectStore.createIndex("username", "username", { unique: true });
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

    const btnClick = document.querySelector("#btn-login");
    if (!btnClick) {
        console.error("Login button not found.");
        return;
    }

    const usernameInput = document.querySelector("#exampleInputUsername1");
    const passwordInput = document.querySelector("#exampleInputPassword1");

    btnClick.addEventListener("click", function (event) {
        event.preventDefault();

        const usernameValue = usernameInput.value;
        const passwordValue = passwordInput.value;

        if (!usernameValue || !passwordValue) {
            alert("Please fill in both username and password.");
            return;
        }

        const loginData = {
            username: usernameValue,
            password: passwordValue,
        };

        console.log("Login data:", loginData);

        fetch("http://localhost:5103/api/User/login", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify(loginData),
        })
            .then((response) => {
                if (!response.ok) {
                    throw new Error("Incorrect username or password.");
                }
                return response.text(); // Pretpostavljamo da API vraća JSON objekt
            })
            .then((data) => {
                // Kod za prijavu korisnika (dohvaćanje iz IndexedDB)
                if (!db) {
                    throw new Error("Database not initialized.");
                }

                const transaction = db.transaction(["users"], "readonly");
                const objectStore = transaction.objectStore("users");
                const index = objectStore.index("username");
                const request = index.get(usernameValue);

                request.onsuccess = function (event) {
                    const user = event.target.result;
                    if (user) {
                        // Provjerite lozinku
                        // Ako koristite hashing, provjerite lozinku protiv hashiranih vrijednosti
                        if (user.password === passwordValue) {
                            // Ovdje se uspoređuju lozinke direktno
                            localStorage.setItem("userId", user.id);
                            console.log("Logged in userId:", user.id);
                            window.location.href =
                                "../LoggedInUser/CreateKahootQuiz.html";
                        } else {
                            alert("Incorrect password.");
                        }
                    } else {
                        alert("User not found in IndexedDB.");
                    }
                };

                request.onerror = function (event) {
                    console.error("Error:", event.target.errorCode);
                    alert("Login failed. Please try again.");
                };
            })
            .catch((error) => {
                console.error("Error:", error);
                alert("Login failed. Check username and password.");
            });
    });
});
