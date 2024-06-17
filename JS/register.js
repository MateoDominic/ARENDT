const btnClick = document.querySelector("#btn-register"); //treba dodati u html ako se bude mjenjao

const username = document.querySelector("#exampleInputUsername1");
const password = document.querySelector("#exampleInputPassword1");

const firstName = document.querySelector("#exampleInputFirstName1");
const email = document.querySelector("#exampleInputEmail1");
const lastName = document.querySelector("#exampleInputLastName1");

document.addEventListener("DOMContentLoaded", function () {
    console.log("I am ready!");

    btnClick.addEventListener("click", function (event) {
        event.preventDefault(); //sprijecavamo defaultno slanje na server nego mi to radimo s JS
        console.log("clicked");

        const loginData = {
            username: username.value,
            password: password.value,
            firstName: firstName.value,
            email: email.value,
            lastName: lastName.value,
        };

        console.log(
            username.value,
            password.value,
            firstName.value,
            email.value,
            lastName.value
        );

        const response = fetch("http://localhost:5103/api/User/register", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify(loginData),
        })
            .then(function (response) {
                if (!response.ok) {
                    throw new Error("response error");
                }
                return response.json();
            })
            .then(function (data) {
               
                location.replace('../LoggedInUser/CreateKahootQuiz.html');

                console.log(data)
            })
            .catch(function (error) {
                console.error("fetch error" + error);
                console.log("FAILURE");
            });
    });
});