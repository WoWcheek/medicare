import SignUpForm from "./SignUpForm";
import "./SignUpPage.scss";

function SignUpPage() {
    const currentYear = 1900 + new Date().getYear();

    return (
        <main>
            <div className="image-container"></div>
            <div className="form-container">
                <SignUpForm />
                <p className="copyright">&copy; {currentYear}</p>
            </div>
        </main>
    );
}

export default SignUpPage;
