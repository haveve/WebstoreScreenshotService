import { useDispatch } from 'react-redux';
import cookieStore from '../behavior/cookie/store';
import { getLogoutAction } from '../behavior/epic';

type Props = {
    showCookieBar: boolean;
    setVisibility: (visible: boolean) => void;
}

const CookieBar = ({ showCookieBar, setVisibility }: Props) => {
    const handleChoice = (choice: boolean) => {
        cookieStore.setCookieConsent(choice);
        setVisibility(false);
    };

    const dispatch = useDispatch();
    const handleLogout = () => dispatch(getLogoutAction());

    if (!showCookieBar)
        return null;

    return (
        <div style={styles.cookieBar}>
            <span>This website uses cookies to enhance your experience. By continuing on our site, you are agreeing to our {<a href='/policy-privacy'>Privacy Policy</a>}</span>
            <div>
                <button style={{ ...styles.button, ...styles.accept }} onClick={() => handleChoice(true)}>
                    Accept essential cookies
                </button>
                <button style={{ ...styles.button, ...styles.decline }} onClick={() => {
                    handleChoice(false);
                    handleLogout();

                }}>
                    Decline
                </button>
            </div>
        </div>
    );
};

const styles = {
    cookieBar: {
        position: 'fixed' as const,
        bottom: 0,
        left: 0,
        width: '100%',
        backgroundColor: '#333',
        color: 'white',
        textAlign: 'center' as const,
        padding: '15px',
        zIndex: 1000,
        display: 'flex',
        justifyContent: 'space-between',
        alignItems: 'center',
    },
    button: {
        margin: '0 10px',
        padding: '10px 20px',
        border: 'none',
        cursor: 'pointer',
    },
    accept: {
        backgroundColor: '#4CAF50',
        color: 'white',
    },
    decline: {
        backgroundColor: '#f44336',
        color: 'white',
    }
};

export default CookieBar;
