import { Route, Routes } from "react-router-dom"
import { Layout } from "./components/layout/layout"
import { LoginPage } from "./pages/login"
import { SignupPage } from "./pages/signup"
import { ArticlePage } from "./pages/article"
import { HomePage } from "./pages/home"
import { ProfilePage } from "./pages/profile"
import { SubscribePage } from "./pages/subscribe"

export function App() {
  return (
    <Routes>
      <Route element={<Layout />}>
        <Route path="/" element={<HomePage />} />
        <Route path="/article/:slug" element={<ArticlePage />} />
        <Route path="/profile" element={<ProfilePage />} />
        <Route path="/subscribe" element={<SubscribePage />} />
      </Route>
      <Route path="/login" element={<LoginPage />} />
      <Route path="/signup" element={<SignupPage />} />
    </Routes>
  )
}

export default App
