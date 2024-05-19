import PropTypes from "prop-types";

function Logo({ logoSize, fontSize }) {
   const flexCssStyle = {
      display: "flex",
      justifyContent: "center",
      alignItems: "center",
      gap: "3%"
   };

   const h1CssStyle = {
      textTransform: "uppercase",
      fontSize: fontSize || "2rem",
      color: "#742b75",
      marginTop: "5px"
   };

   return (
      <div style={flexCssStyle} className="logo-container">
         <img
            src="src/assets/images/logo.png"
            alt="rndvu logo"
            height={logoSize || 50}
         />
           <h1 style={h1CssStyle}>MediCare</h1>
      </div>
   );
}

export default Logo;

Logo.propTypes = {
   logoSize: PropTypes.number,
   fontSize: PropTypes.string
};
