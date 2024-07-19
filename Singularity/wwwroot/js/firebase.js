// Import the functions you need from the SDKs you need
import { initializeApp } from "https://www.gstatic.com/firebasejs/10.12.4/firebase-app.js";
import {
    getAuth, onAuthStateChanged, browserLocalPersistence, setPersistence, createUserWithEmailAndPassword,
    signInWithEmailAndPassword, sendPasswordResetEmail, signOut
} from 'https://www.gstatic.com/firebasejs/10.12.4/firebase-auth.js';


const firebaseConfig = {
    apiKey: "AIzaSyBng5YuwbN9of_gVycZyAx9tpx3ItO_LpU",
    authDomain: "singularity-music-app.firebaseapp.com",
    projectId: "singularity-music-app",
    storageBucket: "singularity-music-app.appspot.com",
    messagingSenderId: "634757845361",
    appId: "1:634757845361:web:0258477b89f3c0f40de6d0",
    measurementId: "G-6JRPZPZ4L6"
};

// Initialize Firebase
const app = initializeApp(firebaseConfig);

setPersistence(getAuth(app), browserLocalPersistence);

export function FirebaseCreateUserWithEmailAndPassword(email, password) {
    return createUserWithEmailAndPassword(getAuth(app), email, password);
}

export function FirebaseSignInWithEmailAndPassword(email, password) {
    return signInWithEmailAndPassword(getAuth(app), email, password);
}

export function FirebaseGeneratePasswordResetLink(email) {
    return sendPasswordResetEmail(getAuth(app), email);
}

export function FirebaseSignOut() {
    return signOut(getAuth(app));
}

export function getCurrentUser() {
    const user = getAuth(app).currentUser;
    if (user) {
        return user;
    }
    else {
        return null;
    }
}
export function subAuthStateChanged(dotnet) {
    onAuthStateChanged(getAuth(app), (data) => {
        dotnet.invokeMethodAsync("authChanged", data);
    });
}