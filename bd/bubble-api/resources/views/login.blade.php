<!DOCTYPE html>
<html lang="es">
<head>
    <meta charset="UTF-8">
    <title>Login - Buble</title>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css">
    
    <style>
        body {
            margin: 0;
            padding: 0;
            font-family: 'Arial', sans-serif;
            background-color: #3b4cca;
            /* Ajuste de fondo para que el GIF cubra todo */
            background-image: url('https://freight.cargo.site/t/original/i/ead9616a26eff00700fd6053e96fd614141f6c9a5ec9ea6080d0fc9a726119d7/swirls-1.gif');
            background-size: cover;
            background-position: center;
            height: 100vh;
            display: flex;
            justify-content: center;
            align-items: center;
        }

        .main-container {
            display: flex;
            width: 85%;
            max-width: 900px; 
            align-items: center;
        }

        .welcome-section {
            flex: 1;
            color: white;
            padding: 20px;
        }

        .welcome-section h1 {
            font-size: 60px; 
            line-height: 0.9;
            margin: 0;
            font-weight: 800;
        }

        .login-card {
            background: white;
            width: 380px; 
            padding: 40px; 
            border-radius: 40px; 
            box-shadow: 0 20px 40px rgba(0,0,0,0.3);
        }

        .card-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 30px;
        }

        .card-header h2 {
            font-size: 28px; 
            margin: 0;
            color: #1a1a1a;
        }

        .logo-img {
            width: 50px; 
            height: 50px;
            object-fit: contain;
        }

        .input-box {
            position: relative;
            margin-bottom: 15px;
        }

        .input-box i {
            position: absolute;
            left: 18px;
            top: 50%;
            transform: translateY(-50%);
            color: #999;
            font-size: 14px;
        }

        .input-box input {
            width: 100%;
            padding: 14px 15px 14px 45px; 
            border: none;
            background-color: #f1f3f6;
            border-radius: 15px;
            font-size: 14px;
            outline: none;
            box-sizing: border-box;
        }

        .options {
            display: flex;
            justify-content: space-between;
            font-size: 12px;
            color: #888;
            margin-bottom: 25px;
        }

        .btn-main {
            width: 100%;
            padding: 14px;
            border: none;
            background-color: #000000;
            color: white;
            font-weight: bold;
            font-size: 15px;
            border-radius: 15px;
            cursor: pointer;
            transition: 0.3s;
        }

        .btn-main:hover {
            background-color: #333;
        }

        .divider {
            text-align: center;
            margin: 20px 0;
            color: #ccc;
            font-size: 12px;
        }

        .btn-secondary {
            display: block;
            text-align: center;
            width: 100%;
            padding: 14px;
            background-color: #f1f3f6;
            color: #555;
            text-decoration: none;
            font-weight: bold;
            border-radius: 15px;
            font-size: 14px;
            box-sizing: border-box;
        }

        @media (max-width: 768px) {
            .welcome-section { display: none; }
            .login-card { margin: 0 auto; }
        }
    </style>
</head>
<body>

    <div class="main-container">
        <div class="welcome-section">
            <h1>Bienvenido<br>de nuevo!</h1>
        </div>

        <div class="login-card">
            <div class="card-header">
                <h2>Inicio de sesión</h2>
                <img src="https://i.gifer.com/LCPW.gif" alt="Logo" class="logo-img">
            </div>

            <form action="{{ route('login.post') }}" method="POST">
                @csrf
                <div class="input-box">
                    <i class="fas fa-user"></i>
                    <input type="email" name="email" placeholder="Correo electrónico" required>
                </div>

                <div class="input-box">
                    <i class="fas fa-lock"></i>
                    <input type="password" name="password" placeholder="Contraseña" required>
                </div>

                <div class="options">
                    <label><input type="checkbox"> Recuérdame</label>
                    <a href="#" style="text-decoration: none; color: #888;">¿Olvidaste tu contraseña?</a>
                </div>

                <button type="submit" class="btn-main">Iniciar sesión</button>

                <div class="divider">O</div>

                <a href="{{ route('registro') }}" class="btn-secondary">Regístrate</a>
            </form>
        </div>
    </div>

</body>
</html>