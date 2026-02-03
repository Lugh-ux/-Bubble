<!DOCTYPE html>
<html>
<head>
    <title>Buble Web</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet">
</head>
<body class="bg-light">
    <nav class="navbar navbar-dark bg-primary mb-4">
        <div class="container">
            <a class="navbar-brand" href="#">Buble Wwb</a>
        </div>
    </nav>

    <div class="container">
        <div class="row">
            <div class="col-md-8 mx-auto">
                <h3>Actividad Reciente</h3>
                @foreach($burbujas as $b)
                    <div class="card mb-3 shadow-sm">
                        <div class="card-body">
                            <h5 class="card-title">Burbuja de {{ $b->user->name }}</h5>
                            <p class="card-text text-muted">
                                Ubicación: {{ $b->latitude }}, {{ $b->longitude }}
                            </p>
                            <small class="text-secondary">Puesta el: {{ $b->created_at }}</small>
                        </div>
                    </div>
                @endforeach
            </div>
        </div>
    </div>
</body>
</html>