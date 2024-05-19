import SignInForm from "./SignInForm";
import "./SignUpPage.scss";

function SignInPage() {
   const currentYear = 1900 + new Date().getYear();

   return (
      <main>
         <div className="image-container"></div>
         <div className="form-container">
            <SignInForm />
            <p className="copyright">&copy; {currentYear}</p>
         </div>
      </main>
   );
}

export default SignInPage;
