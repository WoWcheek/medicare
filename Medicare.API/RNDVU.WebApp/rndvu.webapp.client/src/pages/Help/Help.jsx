import Logo from "../../components/Logo";
import "./Help.scss";

function Help() {
    return (
        <div className="container">
            <div className="d-flex justify-content-center w-100">
                <Logo fontSize={"5rem"} logoSize={80} />
            </div>
            <hr className="help-hr" />
            <section className="showcase">
                <div className="container">
                    <h1>
                        Your trusted partner in finding <br />
                        specialized doctors
                    </h1>
                    <p>
                        Conveniently schedule appointments with top-rated
                        medical professionals tailored to your specific needs.
                    </p>
                </div>
            </section>

            <section className="content-section container help">
                <h2>Why choose Medicare?</h2>
                <p>
                    <strong>Comprehensive Doctor Directory:</strong> Our
                    extensive database includes specialists from all fields of
                    medicine, ensuring you find the right doctor for your health
                    concerns. Whether you need a cardiologist, dermatologist,
                    pediatrician, or any other specialist, Medicare has got you
                    covered.
                </p>
                <p>
                    <strong>Easy Appointment Scheduling:</strong> Say goodbye to
                    long waits and complicated booking processes. With Medicare,
                    you can quickly and easily book appointments with your
                    chosen doctor at a time that suits you.
                </p>
                <p>
                    <strong>Verified Reviews and Ratings:</strong> Make informed
                    decisions with access to genuine patient reviews and
                    ratings. Learn from others&apos; experiences to select the
                    best healthcare provider for you and your family.
                </p>
                <p>
                    <strong>User-Friendly Interface:</strong> Our intuitive
                    platform makes it simple to search for doctors, compare
                    their qualifications and reviews, and schedule appointments:
                    all in a few clicks.
                </p>
                <p>
                    <strong>Secure and Confidential:</strong> We prioritize your
                    privacy and security. Your personal information is protected
                    with state-of-the-art encryption, ensuring your data remains
                    confidential.
                </p>
                <p>
                    <strong>24/7 Access:</strong> Healthcare needs can arise at
                    any time. Medicare is available around the clock, giving you
                    the flexibility to find and book appointments whenever you
                    need them.
                </p>

                <h2>How it works:</h2>
                <p>
                    <strong>Search for Specialists:</strong> Use our easy search
                    function to find doctors by specialty, location, and
                    availability.
                </p>
                <p>
                    <strong>Compare Options:</strong> Review detailed profiles,
                    patient reviews, and ratings to choose the best doctor for
                    your needs.
                </p>
                <p>
                    <strong>Book Appointments:</strong> Select a convenient time
                    slot and book your appointment instantly.
                </p>
                <p>
                    <strong>Receive Confirmation:</strong> Get immediate
                    confirmation of your appointment and reminders to keep you
                    on track.
                </p>

                <h2>Join the Medicare community today</h2>
                <p>
                    Experience the convenience and reliability of Medicare.
                    Start your journey towards better health with just a few
                    clicks. Visit our website or download our app to get
                    started. Your perfect doctor is just a search away with
                    Medicare!
                </p>
            </section>
        </div>
    );
}

export default Help;
