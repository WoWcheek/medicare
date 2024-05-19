import { PropTypes } from "prop-types";
import { Link } from "@mui/material";

function ButtonLink({ fontSize = "1.18rem", disabled, children, onClick }) {
   if (disabled)
      return (
         <Link
            fontSize={fontSize}
            textAlign="center"
            underline="none"
            component="button"
            disabled
            onClick={onClick}
         >
            {children}
         </Link>
      );

   return (
      <Link
         fontSize={fontSize}
         textAlign="center"
         underline="hover"
         component="button"
         onClick={onClick}
         >
         {children}
      </Link>
   );
}

ButtonLink.propTypes = {
   fontSize: PropTypes.string,
   disabled: PropTypes.bool,
   children: PropTypes.string
};

export default ButtonLink;
