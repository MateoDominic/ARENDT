const btnClick = document.querySelector("#btnLogin"); //promjeni id u html za btn 
const username = document.querySelector("#exampleInputUsername1");
const password = document.querySelector("#exampleInputPassword1");

document.addEventListener("DOMContentLoaded", function () {
    console.log("I am ready!"); 

    btnClick.addEventListener("click", function (event) {
        event.preventDefault(); //sprijecavamo defaultno slanje na server nego mi to radimo s JS

        console.log("clicked");

        const usernameValue = username.value;
        const passwordValue = password.value;

        if (!usernameValue || !passwordValue) {
            alert("popuni podatke za prijavu");
            return;
        }

        const loginData = {
            username: usernameValue,
            password: passwordValue,
        };

        console.log(usernameValue, passwordValue);

        const response = fetch("http://localhost:5103/api/User/login", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify(loginData),
        })
        
            .then(function (response) {
                if (!response.ok) {
                    throw new Error("Pogrešno korisnicko ime ili lozinka.");
                }
                return response.text();//mjenjano u .text iz .json()
            })

            .then(function (data) {
                console.log("Token:", data.token);
                localStorage.setItem("token",data.token);

                
                location.replace('../LoggedInUser/CreateKahootQuiz.html');

               
                console.log("Sucessiful login!");
            })
            .catch(function (error) {
                console.error("Error: ", error);
                console.log("FAILURE");
            });
    });
});