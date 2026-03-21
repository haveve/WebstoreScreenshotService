import { Field } from "formik";
import {
    TextField
} from "@mui/material";
import { FieldProps } from "./types";
import { formatValidate } from "./helpers";
import { memo } from "react";

/**
 * Wrapper for MUI TextField
 */
const FormikTextField = ({ name, label, schema, ...props }: FieldProps) => {
    return (
        <Field name={name} validate={formatValidate(schema)}>
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