import SignInForm from "./SignInForm";
import "./SignUpPage.scss";

function SignInPage() {
    const currentYear = 1900 + new Date().getYear();

    return (
        <main className="sign-up">
            <div className="image-container"></div>
            <div className="form-container">
                <SignInForm />
                <p className="copyright">&copy; {currentYear} Medicare</p>
            </div>
        </main>
    );
}

export default SignInPage;
