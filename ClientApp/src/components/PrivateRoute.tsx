import { useNavigate } from "react-router-dom";

type PrivateRouteProps = {
    validate: () => boolean;
    toPath: string;
    element: React.ReactElement;
}

const PrivateRoute = ({ element, toPath, validate }: PrivateRouteProps) => {
    const navigate = useNavigate();

    if (validate())
        return element;

    navigate(toPath, { replace: true });
    return null;
};

export default PrivateRoute;