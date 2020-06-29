package ej2.webservices;

import static org.junit.jupiter.api.Assertions.assertEquals;
import java.net.URI;
import org.junit.jupiter.api.Test;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.context.SpringBootTest;
import org.springframework.boot.test.web.client.TestRestTemplate;
import org.springframework.http.HttpHeaders;
import org.springframework.http.HttpStatus;
import org.springframework.http.RequestEntity;
import org.springframework.http.ResponseEntity;

@SpringBootTest(webEnvironment = SpringBootTest.WebEnvironment.RANDOM_PORT)
public class WordEditorControllerTests {

	@Autowired
	private TestRestTemplate restTemplate;

	@Test
	public void corsWithAnnotation() throws Exception {
		ResponseEntity<String> entity = this.restTemplate.exchange(
				RequestEntity.get(uri("/api/wordeditor/test")).header(HttpHeaders.ORIGIN, "http://localhost:9090").build(),
				String.class);
		assertEquals(HttpStatus.OK, entity.getStatusCode());
		assertEquals("*", entity.getHeaders().getAccessControlAllowOrigin());
		String message = entity.getBody();
		assertEquals("{\"sections\":[{\"blocks\":[{\"inlines\":[{\"text\":\"Hello World\"}]}]}]}", message);
	}

//	@Test
//	public void corsWithJavaconfig() {
//		ResponseEntity<Greeting> entity = this.restTemplate.exchange(RequestEntity.get(uri("/greeting-javaconfig"))
//				.header(HttpHeaders.ORIGIN, "http://localhost:9000").build(), Greeting.class);
//		assertEquals(HttpStatus.OK, entity.getStatusCode());
//		assertEquals("http://localhost:9000", entity.getHeaders().getAccessControlAllowOrigin());
//		Greeting greeting = entity.getBody();
//		assertEquals("Hello, World!", greeting.getContent());
//	}

	private URI uri(String path) {
		return restTemplate.getRestTemplate().getUriTemplateHandler().expand(path);
	}

}
