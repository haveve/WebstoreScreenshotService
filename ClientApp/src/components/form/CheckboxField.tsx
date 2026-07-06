import { Field } from "formik";
import {
    Checkbox,
    FormControlLabel,
    FormHelperText,
    FormControl,
    CheckboxProps as BaseCheckboxProps
} from "@mui/material";
import { FieldProps } from "./types";
import { memo, ReactNode } from "react";

/**
 * Wrapper for MUI TextField
 */

type CheckboxProps = BaseCheckboxProps & {
  name: string;
  label: ReactNode;
}

const CheckboxField = ({ name, label, ...props }: CheckboxProps) => {
    return (
        <Field name={name}>
            {({ field, meta }: FieldProps) => (
                <FormControl error={meta.touched && Boolean(meta.error)}>
                    <FormControlLabel
                        control={
                            <Checkbox
                                {...field}
                                {...props}
                                checked={field.value}
                            />
                        }
                        label={label}
                    />
                    {meta.touched && meta.error && (
                        <FormHelperText>{meta.error}</FormHelperText>
                    )}
                </FormControl>
            )}
        </Field>
    );
};

export default memo(CheckboxField);
