import { Field } from "formik";
import {
    TextField,
    TextFieldProps as BaseTextFieldProps
} from "@mui/material";
import { FieldProps } from "./types";
import { memo, ReactNode } from "react";

/**
 * Wrapper for MUI TextField
 */

type TextFieldProps = BaseTextFieldProps & {
    name: string;
    label: ReactNode;
}

const FormikTextField = ({ name, label, ...props }: TextFieldProps) => {
    return (
        <Field name={name}>
            {({ field, meta }: FieldProps) => (
                <TextField
                    {...field}
                    {...props}
                    label={label}
                    error={meta.touched && Boolean(meta.error)}
                    helperText={meta.touched && meta.error}
                    fullWidth
                />
            )}
        </Field>
    );
};

export default memo(FormikTextField);