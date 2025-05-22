import './App.css'
import Nav from './components/nav/Nav'
import ModpackList from "./components/modpacks/ModpackList"
import ModpackView from './components/modpacks/ModpackView'
import LoginForm from './components/profile/LoginForm.jsx'
import NewModpack from './components/modpacks/NewModpack.jsx'
import ProfileView from './components/profile/ProfileView.jsx';
import EditProfile from './components/profile/EditProfile.jsx'
import store from "./store.js";
import { BrowserRouter, Route, Routes } from 'react-router'

function App() {

  let state = store();
  let { curUser } = state;

  function verifyUser() {
    
  }

  if(!curUser) {
    return (
      <main> 
        <LoginForm />
      </main>
    );
  }

  return <BrowserRouter>
      <Routes>
        <Route path="/" element={
          <main className="w-full min-h-[100vh] flex flex-col items-center justify-center">
            <Nav /> 
            <ModpackList />  
          </main>} />

        <Route path="/modpack_view" element={
          <main className="w-full min-h-[100vh] flex flex-col items-center justify-center">
            <Nav /> 
            <ModpackView />  
          </main>} />

        <Route path="/profile" element={
          <main className="w-full min-h-[100vh] flex flex-col items-center justify-center">
            <Nav /> 
            <ProfileView />  
          </main>} />
        
          <Route path="/edit_profile" element={
            <main className="w-full min-h-[100vh] flex flex-col items-center justify-center">
              <Nav /> 
              <EditProfile />  
            </main>} />

            <Route path="/create_modpack" element={
              <main className="w-full min-h-[100vh] flex flex-col items-center justify-center">
                <Nav /> 
                <NewModpack />  
              </main>} />
      </Routes>
  </BrowserRouter>
}

export default App
