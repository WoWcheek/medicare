import { Button } from "@mui/material";
import PropTypes from "prop-types";

function SubmitButton({ variant, onClick, children }) {
   return (
      <Button fullWidth type="submit" variant={variant} onClick={onClick}>
         {children}
      </Button>
   );
}

export default SubmitButton;

SubmitButton.propTypes = {
   variant: PropTypes.string,
   onClick: PropTypes.func,
   children: PropTypes.string
};
