package com.example.bubble;

import android.content.Intent;
import android.os.Bundle;
import android.widget.Button;
import android.widget.EditText;
import android.widget.TextView;
import android.widget.Toast;

import androidx.activity.EdgeToEdge;
import androidx.appcompat.app.AppCompatActivity;
import androidx.core.graphics.Insets;
import androidx.core.view.ViewCompat;
import androidx.core.view.WindowInsetsCompat;

import com.google.android.material.textfield.TextInputEditText;
import com.google.android.material.textfield.TextInputLayout;

public class Registro extends AppCompatActivity {

    Button btRegistro;
    TextInputEditText txtUsuario, txtCorreo, txtContraseña, txtContraseñaConf;


    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        EdgeToEdge.enable(this);
        setContentView(R.layout.activity_registro);
        ViewCompat.setOnApplyWindowInsetsListener(findViewById(R.id.main), (v, insets) -> {
            Insets systemBars = insets.getInsets(WindowInsetsCompat.Type.systemBars());
            v.setPadding(systemBars.left, systemBars.top, systemBars.right, systemBars.bottom);
            return insets;
        });

        TextView login = findViewById(R.id.txtIniciarsesion);
        login.setOnClickListener(v -> {
            startActivity(new Intent(this, LogIn.class));

        });

        txtUsuario = findViewById(R.id.createBoxUsuario);
        txtCorreo = findViewById(R.id.createBoxCorreo);
        txtContraseña = findViewById(R.id.createBoxContraseña);
        txtContraseñaConf = findViewById(R.id.createBoxConfContraseña);
        btRegistro = findViewById(R.id.btnRegistro);


        btRegistro.setOnClickListener(v -> {
            String user = txtUsuario.getText().toString().trim();
            String email = txtCorreo.getText().toString().trim();
            String pass = txtContraseña.getText().toString().trim();
            String confirm = txtContraseñaConf.getText().toString().trim();

            if (user.isEmpty() || email.isEmpty() || pass.isEmpty()) {
                Toast.makeText(this, "Rellena todos los campos", Toast.LENGTH_SHORT).show();
                return;
            }

            if (!pass.equals(confirm)) {
                Toast.makeText(this, "Las contraseñas no coinciden", Toast.LENGTH_SHORT).show();

            } else {
                APIRest api = new APIRest();
                api.registrarUsuario(user, email, pass, this);
            }


        });


    }
}