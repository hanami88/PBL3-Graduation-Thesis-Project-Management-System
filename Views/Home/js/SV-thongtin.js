const changepasswork = document.querySelector(".change-password");
const formchangepass = document.querySelector(".formchange-passwork");

changepasswork.addEventListener("click", () => {
  formchangepass.classList.toggle("hidden");
  changepasswork.classList.toggle("on");
});
