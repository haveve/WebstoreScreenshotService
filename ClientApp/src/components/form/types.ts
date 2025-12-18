import { ReactNode } from "react";
import * as Yup from "yup";

export type FieldProps = React.InputHTMLAttributes<HTMLInputElement> & {
    name: string;
    label: ReactNode;
    schema?: Yup.Schema<any>; // 👈 yup schema for this field
    [key: string]: any;
};