import { ReactNode } from "react";

export type FieldProps = React.InputHTMLAttributes<HTMLInputElement> & {
    name: string;
    label: ReactNode;
    [key: string]: any;
};