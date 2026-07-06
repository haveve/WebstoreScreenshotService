import { Field } from "formik";
import {
    Switch,
    FormControlLabel,
    FormHelperText,
    FormControl,
    SwitchProps as BaseSwitchProps
} from "@mui/material";
import { FieldProps } from "./types";
import { memo, ReactNode } from "react";

/**
 * Wrapper for MUI TextField
 */

type SwitchProps = BaseSwitchProps & {
    name: string;
    label: ReactNode;
}

const CheckboxField = ({ name, label, ...props }: SwitchProps) => {
    return (
        <Field name={name}>
            {({ field, meta }: FieldProps) => (
                <FormControl error={meta.touched && Boolean(meta.error)}>
                    <FormControlLabel
                        control={
                            <Switch
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
