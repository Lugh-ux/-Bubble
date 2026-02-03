<!DOCTYPE html>
<html>
<head>
    <title>Registro - Buble</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet">
</head>
<body class="bg-light">
    <div class="container mt-5">
        <div class="row justify-content-center">
            <div class="col-md-4 card shadow p-4">
                <h2 class="text-center">Crear Cuenta</h2>
                <form action="{{ route('registro.post') }}" method="POST">
                    @csrf
                    <div class="mb-3">
                        <label>Nombre</label>
                        <input type="text" name="name" class="form-control" required>
                    </div>
                    <div class="mb-3">
                        <label>Email</label>
                        <input type="email" name="email" class="form-control" required>
                    </div>
                    <div class="mb-3">
                        <label>Contraseña</label>
                        <input type="password" name="password" class="form-control" required>
                    </div>
                    <button type="submit" class="btn btn-success w-100">Registrarme</button>
                    <a href="{{ route('login') }}" class="d-block mt-3 text-center">¿Ya tienes cuenta? Logueate</a>
                </form>
            </div>
        </div>
    </div>
</body>
</html>