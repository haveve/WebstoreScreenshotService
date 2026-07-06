import * as Yup from "yup";

export const formatValidate = (schema?: Yup.Schema<any>) => {
    if (!schema)
        return undefined;

    return async (value: any) => {
        if (!schema) return undefined;
        try {
            await schema.validate(value);
            return undefined;
        } catch (err: any) {
            return err.message;
        }
    };
}