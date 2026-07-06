import { Field } from "formik";
import { TextField, TextFieldProps as BaseTextFieldProps } from "@mui/material";
import { FieldProps } from "./types";
import { memo, ReactNode } from "react";

/**
 * Wrapper for MUI TextField
 */

type ColorPickerProps = Omit<BaseTextFieldProps, 'type'> & {
  name: string;
  label: ReactNode;
}

/**
 * Wrapper for a color picker using MUI and Formik
 */
const FormikColorPicker = ({ name, label, ...props }: ColorPickerProps) => {
  return (
    <Field name={name}>
      {({ field, meta }: FieldProps) => (
        <TextField
          {...field}
          {...props}
          label={label}
          type="color"
          error={meta.touched && Boolean(meta.error)}
          helperText={meta.touched && meta.error}
          fullWidth
        />
      )}
    </Field>
  );
};

export default memo(FormikColorPicker);