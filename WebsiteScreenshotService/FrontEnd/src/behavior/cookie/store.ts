const CONSENT_KEY = process.env.REACT_APP_COOKIE_CONSENT_KEY || "cookie_consent";

enum CookieChoice {
    ACCEPTED = 'accepted',
    DECLINED = 'declined'
}

export default {
    getCookieConsent: (): boolean | null => {
        const consent = localStorage.getItem(CONSENT_KEY);
        return consent === null ? null : consent === CookieChoice.ACCEPTED;
    },

    declinedCookieConsent: (): boolean => {
        return localStorage.getItem(CONSENT_KEY) === CookieChoice.DECLINED;
    },

    setCookieConsent: (choice: boolean): void => {
        localStorage.setItem(CONSENT_KEY, choice ? CookieChoice.ACCEPTED : CookieChoice.DECLINED);
    },
    removeCookieConsent: (): void => {
        localStorage.removeItem(CONSENT_KEY);
    }
}