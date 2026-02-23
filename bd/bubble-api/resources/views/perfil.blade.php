<!DOCTYPE html>
<html lang="es">

<head>
    <meta charset="UTF-8">
    <title>Editar Perfil - Bubble</title>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css">
    <link href="https://fonts.googleapis.com/css2?family=Roboto+Mono:wght@400;700&display=swap" rel="stylesheet">
    <style>
        body {
            margin: 0;
            padding: 0;
            font-family: 'Roboto Mono', monospace;
            background-color: #131313;
            background-image: url('https://freight.cargo.site/t/original/i/ead9616a26eff00700fd6053e96fd614141f6c9a5ec9ea6080d0fc9a726119d7/swirls-1.gif');
            background-size: cover;
            background-attachment: fixed;
            height: 100vh;
            display: flex;
            justify-content: center;
            align-items: center;
        }

        .panel-editar {
            background: rgba(255, 255, 255, 0.95);
            backdrop-filter: blur(10px);
            border-radius: 30px;
            padding: 40px;
            box-shadow: 0 10px 40px rgba(0, 0, 0, 0.5);
            width: 100%;
            max-width: 400px;
            text-align: center;
        }

        .panel-editar h2 {
            margin-top: 0;
            color: #131313;
        }

        .avatar-preview {
            width: 120px;
            height: 120px;
            border-radius: 50%;
            border: 4px solid #3b4cca;
            object-fit: cover;
            margin-bottom: 20px;
        }

        .form-group {
            margin-bottom: 20px;
            text-align: left;
        }

        .form-group label {
            display: block;
            font-weight: bold;
            margin-bottom: 8px;
            color: #333;
        }

        .form-control {
            width: 100%;
            padding: 12px;
            border: 2px solid #ddd;
            border-radius: 10px;
            font-family: 'Roboto Mono', monospace;
            box-sizing: border-box;
            transition: border-color 0.3s;
        }

        .form-control:focus {
            outline: none;
            border-color: #3b4cca;
        }

        .btn-submit {
            background-color: #3b4cca;
            color: white;
            border: none;
            padding: 15px;
            width: 100%;
            border-radius: 10px;
            font-family: 'Roboto Mono', monospace;
            font-weight: bold;
            font-size: 16px;
            cursor: pointer;
            transition: background 0.3s;
            margin-top: 10px;
        }

        .btn-submit:hover {
            background-color: #2a3899;
        }

        .btn-back {
            display: block;
            margin-top: 20px;
            color: #666;
            text-decoration: none;
            font-size: 14px;
        }

        .btn-back:hover {
            text-decoration: underline;
        }

        .file-input-wrapper {
            position: relative;
            overflow: hidden;
            display: inline-block;
            margin-bottom: 20px;
        }

        .file-input-wrapper input[type=file] {
            font-size: 100px;
            position: absolute;
            left: 0;
            top: 0;
            opacity: 0;
            cursor: pointer;
        }

        .btn-file {
            background-color: #eee;
            color: #333;
            padding: 8px 15px;
            border-radius: 20px;
            font-size: 12px;
            font-weight: bold;
            border: 1px solid #ddd;
        }
    </style>
</head>

<body>

    <div class="panel-editar">
        <h2>Editar Perfil</h2>

        <form action="/guardar-mi-perfil-nuevo" method="POST" enctype="multipart/form-data">
            @csrf

            @if ($errors->any())
                <div style="background: #ff4d4d; color: white; padding: 15px; border-radius: 10px; margin-bottom: 20px;">
                    <ul style="margin: 0; padding-left: 20px;">
                        @foreach ($errors->all() as $error)
                            <li>{{ $error }}</li>
                        @endforeach
                    </ul>
                </div>
            @endif

            <img src="{{ Auth::user()->avatar ? asset('storage/' . Auth::user()->avatar) : 'https://ui-avatars.com/api/?name=' . urlencode(Auth::user()->name) . '&background=3b4cca&color=fff' }}" 
                 alt="Avatar" class="avatar-preview" id="imgPreview">
            <br>
            
            <div class="file-input-wrapper">
                <button type="button" class="btn-file"><i class="fas fa-camera"></i> Cambiar Foto</button>
                <input type="file" name="avatar" id="avatarInput" accept="image/*" onchange="previewImage(event)">
            </div>

            <div class="form-group">
                <label for="name">Nombre de usuario</label>
                <input type="text" id="name" name="name" class="form-control" value="{{ Auth::user()->name }}" required>
            </div>

            <button type="submit" class="btn-submit">Guardar Cambios</button>
        </form>

        <a href="{{ url('/muro') }}" class="btn-back"><i class="fas fa-arrow-left"></i> Volver al mapa</a>
    </div>

    <script>
        function previewImage(event) {
            var input = event.target;
            var preview = document.getElementById('imgPreview');

            if (input.files && input.files[0]) {
                var reader = new FileReader();
                reader.onload = function(e) {
                    preview.src = e.target.result;
                }
                reader.readAsDataURL(input.files[0]);
            }
        }
    </script>
</body>

</html>
